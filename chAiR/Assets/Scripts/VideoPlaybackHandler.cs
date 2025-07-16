using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.IO;
using TMPro;
using NativeGalleryNamespace;

public class VideoPlaybackHandler : MonoBehaviour
{
    [Header("Video Player Setup")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RenderTexture videoRenderTexture;
    
    [Header("UI Components")]
    [SerializeField] private GameObject videoPlayerPanel;
    [SerializeField] private RawImage videoDisplay;
    [SerializeField] private Button playPauseButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button selectVideoButton;
    [SerializeField] private Button closePlayerButton;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text videoTimeText;
    [SerializeField] private TMP_Text videoStatusText;
    [SerializeField] private Image playPauseIcon;
    
    [Header("Icons")]
    [SerializeField] private Sprite playIcon;
    [SerializeField] private Sprite pauseIcon;
    
    [Header("File Browser")]
    [SerializeField] private GameObject fileBrowserPanel;
    [SerializeField] private Transform fileBrowserContent;
    [SerializeField] private GameObject videoFileItemPrefab;
    [SerializeField] private Button closeBrowserButton;
    [SerializeField] private TMP_Text browserStatusText;
    
    private bool isPlaying = false;
    private bool isDraggingProgress = false;
    private string currentVideoPath = "";
    private List<string> videoFiles = new List<string>();
    
    // Supported video formats
    private readonly string[] supportedFormats = { ".mp4", ".mov", ".avi", ".mkv", ".webm" };
    
    private void Awake()
    {
        InitializeVideoPlayer();
        SetupUI();
    }
    
    private void InitializeVideoPlayer()
    {
        if (videoPlayer == null)
        {
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
        }
        
        // Create render texture if not assigned
        if (videoRenderTexture == null)
        {
            videoRenderTexture = new RenderTexture(1920, 1080, 0);
        }
        
        // Configure video player
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = videoRenderTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        
        // Set video display texture
        if (videoDisplay != null)
        {
            videoDisplay.texture = videoRenderTexture;
        }
        
        // Subscribe to video player events
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.started += OnVideoStarted;
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.errorReceived += OnVideoError;
    }
    
    private void SetupUI()
    {
        // Hide panels initially
        if (videoPlayerPanel != null)
        {
            videoPlayerPanel.SetActive(false);
        }
        
        if (fileBrowserPanel != null)
        {
            fileBrowserPanel.SetActive(false);
        }
        
        // Setup button listeners
        if (playPauseButton != null)
        {
            playPauseButton.onClick.AddListener(TogglePlayPause);
        }
        
        if (stopButton != null)
        {
            stopButton.onClick.AddListener(StopVideo);
        }
        
        if (selectVideoButton != null)
        {
            selectVideoButton.onClick.AddListener(OpenFileBrowser);
        }
        
        if (closePlayerButton != null)
        {
            closePlayerButton.onClick.AddListener(CloseVideoPlayer);
        }
        
        if (closeBrowserButton != null)
        {
            closeBrowserButton.onClick.AddListener(CloseFileBrowser);
        }
        
        // Setup sliders
        if (progressSlider != null)
        {
            progressSlider.onValueChanged.AddListener(OnProgressSliderChanged);
        }
        
        if (volumeSlider != null)
        {
            volumeSlider.value = 1f;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        
        UpdatePlayPauseIcon();
        UpdateVideoStatus("No video selected");
    }
    
    public void OpenVideoPlayer()
    {
        if (videoPlayerPanel != null)
        {
            videoPlayerPanel.SetActive(true);
        }
    }
    
    public void CloseVideoPlayer()
    {
        StopVideo();
        if (videoPlayerPanel != null)
        {
            videoPlayerPanel.SetActive(false);
        }
    }
    
    public void OpenFileBrowser()
    {
        StartCoroutine(LoadVideoFiles());
    }
    
    public void CloseFileBrowser()
    {
        if (fileBrowserPanel != null)
        {
            fileBrowserPanel.SetActive(false);
        }
    }
    
    private IEnumerator LoadVideoFiles()
    {
        if (fileBrowserPanel != null)
        {
            fileBrowserPanel.SetActive(true);
        }
        
        UpdateBrowserStatus("Scanning for videos...");
        
        videoFiles.Clear();
        
        // Clear existing file items
        if (fileBrowserContent != null)
        {
            foreach (Transform child in fileBrowserContent)
            {
                Destroy(child.gameObject);
            }
        }
        
        yield return new WaitForEndOfFrame();
        
        // Check common video directories
        List<string> searchPaths = new List<string>
        {
            "/storage/emulated/0/DCIM/Camera/",
            "/storage/emulated/0/Movies/",
            "/storage/emulated/0/Download/",
            "/storage/emulated/0/Pictures/",
            Application.persistentDataPath,
            "/storage/emulated/0/"
        };
        
        int foundVideos = 0;
        
        foreach (string searchPath in searchPaths)
        {
            if (Directory.Exists(searchPath))
            {
                try
                {
                    string[] files = Directory.GetFiles(searchPath, "*.*", SearchOption.AllDirectories);
                    
                    foreach (string file in files)
                    {
                        string extension = Path.GetExtension(file).ToLower();
                        if (System.Array.Exists(supportedFormats, format => format == extension))
                        {
                            videoFiles.Add(file);
                            CreateVideoFileItem(file);
                            foundVideos++;
                            
                            if (foundVideos % 10 == 0)
                            {
                                UpdateBrowserStatus($"Found {foundVideos} videos...");
                                yield return new WaitForEndOfFrame();
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Could not access directory {searchPath}: {e.Message}");
                }
            }
        }
        
        if (foundVideos == 0)
        {
            UpdateBrowserStatus("No video files found. Try recording some videos with ARCore first!");
        }
        else
        {
            UpdateBrowserStatus($"Found {foundVideos} video files");
        }
    }
    
    private void CreateVideoFileItem(string filePath)
    {
        if (videoFileItemPrefab == null || fileBrowserContent == null) return;
        
        GameObject item = Instantiate(videoFileItemPrefab, fileBrowserContent);
        
        // Get components
        TMP_Text nameText = item.GetComponentInChildren<TMP_Text>();
        Button selectButton = item.GetComponentInChildren<Button>();
        
        if (nameText != null)
        {
            string fileName = Path.GetFileName(filePath);
            string fileSize = GetFileSizeString(filePath);
            nameText.text = $"{fileName}\n{fileSize}";
        }
        
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(() => SelectVideoFile(filePath));
        }
    }
    
    private string GetFileSizeString(string filePath)
    {
        try
        {
            FileInfo fileInfo = new FileInfo(filePath);
            long bytes = fileInfo.Length;
            
            if (bytes < 1024)
                return $"{bytes} B";
            else if (bytes < 1024 * 1024)
                return $"{bytes / 1024:F1} KB";
            else if (bytes < 1024 * 1024 * 1024)
                return $"{bytes / (1024 * 1024):F1} MB";
            else
                return $"{bytes / (1024 * 1024 * 1024):F1} GB";
        }
        catch
        {
            return "Unknown size";
        }
    }
    
    public void SelectVideoFile(string filePath)
    {
        currentVideoPath = filePath;
        CloseFileBrowser();
        LoadVideo(filePath);
    }
    
    private void LoadVideo(string videoPath)
    {
        if (videoPlayer == null || string.IsNullOrEmpty(videoPath))
        {
            UpdateVideoStatus("Error: Invalid video path");
            return;
        }
        
        UpdateVideoStatus("Loading video...");
        
        // Use file:// protocol for local files
        string url = videoPath.StartsWith("file://") ? videoPath : $"file://{videoPath}";
        
        videoPlayer.url = url;
        videoPlayer.Prepare();
    }
    
    private void OnVideoPrepared(VideoPlayer vp)
    {
        UpdateVideoStatus($"Video loaded: {Path.GetFileName(currentVideoPath)}");
        
        if (progressSlider != null)
        {
            progressSlider.maxValue = (float)vp.frameCount;
            progressSlider.value = 0;
        }
        
        UpdateTimeDisplay();
    }
    
    private void OnVideoStarted(VideoPlayer vp)
    {
        isPlaying = true;
        UpdatePlayPauseIcon();
        UpdateVideoStatus("Playing");
    }
    
    private void OnVideoFinished(VideoPlayer vp)
    {
        isPlaying = false;
        UpdatePlayPauseIcon();
        UpdateVideoStatus("Video finished");
        
        if (progressSlider != null)
        {
            progressSlider.value = progressSlider.maxValue;
        }
    }
    
    private void OnVideoError(VideoPlayer vp, string message)
    {
        UpdateVideoStatus($"Error: {message}");
        Debug.LogError($"Video Error: {message}");
    }
    
    public void TogglePlayPause()
    {
        if (videoPlayer == null || !videoPlayer.isPrepared) return;
        
        if (isPlaying)
        {
            videoPlayer.Pause();
            isPlaying = false;
            UpdateVideoStatus("Paused");
        }
        else
        {
            videoPlayer.Play();
            isPlaying = true;
            UpdateVideoStatus("Playing");
        }
        
        UpdatePlayPauseIcon();
    }
    
    public void StopVideo()
    {
        if (videoPlayer == null) return;
        
        videoPlayer.Stop();
        isPlaying = false;
        
        if (progressSlider != null)
        {
            progressSlider.value = 0;
        }
        
        UpdatePlayPauseIcon();
        UpdateVideoStatus("Stopped");
        UpdateTimeDisplay();
    }
    
    private void OnProgressSliderChanged(float value)
    {
        if (videoPlayer == null || !videoPlayer.isPrepared || isDraggingProgress) return;
        
        videoPlayer.frame = (long)value;
        UpdateTimeDisplay();
    }
    
    public void OnProgressSliderDragStart()
    {
        isDraggingProgress = true;
    }
    
    public void OnProgressSliderDragEnd()
    {
        isDraggingProgress = false;
        if (videoPlayer != null && videoPlayer.isPrepared && progressSlider != null)
        {
            videoPlayer.frame = (long)progressSlider.value;
        }
    }
    
    private void OnVolumeChanged(float value)
    {
        if (videoPlayer != null)
        {
            // Note: VideoPlayer doesn't have direct volume control in Direct audio mode
            // This would need AudioSource integration for volume control
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.volume = value;
            }
        }
    }
    
    private void UpdatePlayPauseIcon()
    {
        if (playPauseIcon != null)
        {
            playPauseIcon.sprite = isPlaying ? pauseIcon : playIcon;
        }
    }
    
    private void UpdateVideoStatus(string status)
    {
        if (videoStatusText != null)
        {
            videoStatusText.text = status;
        }
    }
    
    private void UpdateBrowserStatus(string status)
    {
        if (browserStatusText != null)
        {
            browserStatusText.text = status;
        }
    }
    
    private void UpdateTimeDisplay()
    {
        if (videoTimeText == null || videoPlayer == null || !videoPlayer.isPrepared) return;
        
        double currentTime = videoPlayer.time;
        double totalTime = videoPlayer.length;
        
        string currentTimeStr = FormatTime(currentTime);
        string totalTimeStr = FormatTime(totalTime);
        
        videoTimeText.text = $"{currentTimeStr} / {totalTimeStr}";
    }
    
    private string FormatTime(double timeInSeconds)
    {
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        return $"{minutes:D2}:{seconds:D2}";
    }
    
    private void Update()
    {
        if (videoPlayer != null && videoPlayer.isPrepared && isPlaying)
        {
            // Update progress slider
            if (progressSlider != null && !isDraggingProgress)
            {
                progressSlider.value = videoPlayer.frame;
            }
            
            // Update time display
            UpdateTimeDisplay();
        }
    }
    
    private void OnDestroy()
    {
        // Clean up video player events
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.started -= OnVideoStarted;
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.errorReceived -= OnVideoError;
        }
        
        // Clean up render texture
        if (videoRenderTexture != null)
        {
            videoRenderTexture.Release();
        }
    }
}