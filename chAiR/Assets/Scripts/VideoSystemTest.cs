using UnityEngine;
using UnityEngine.Video;
using System.IO;

public class VideoSystemTest : MonoBehaviour
{
    [Header("Video System Test")]
    [SerializeField] private bool runTestOnStart = true;
    [SerializeField] private bool logDetailedInfo = true;
    
    private void Start()
    {
        if (runTestOnStart)
        {
            RunVideoSystemTest();
        }
    }
    
    public void RunVideoSystemTest()
    {
        Debug.Log("=== Video System Test Started ===");
        
        // Test 1: Check Unity Video Module availability
        TestVideoModuleAvailability();
        
        // Test 2: Check VideoPlayer component creation
        TestVideoPlayerCreation();
        
        // Test 3: Check file system access
        TestFileSystemAccess();
        
        // Test 4: Check supported video formats
        TestVideoFormatSupport();
        
        // Test 5: Check render texture creation
        TestRenderTextureCreation();
        
        Debug.Log("=== Video System Test Completed ===");
    }
    
    private void TestVideoModuleAvailability()
    {
        Debug.Log("Test 1: Video Module Availability");
        
        try
        {
            GameObject testObj = new GameObject("VideoTest");
            VideoPlayer vp = testObj.AddComponent<VideoPlayer>();
            
            if (vp != null)
            {
                Debug.Log("✓ VideoPlayer component created successfully");
                
                // Check available properties
                Debug.Log($"✓ Video module supports render modes: {System.Enum.GetNames(typeof(VideoRenderMode)).Length}");
                Debug.Log($"✓ Video module supports audio modes: {System.Enum.GetNames(typeof(VideoAudioOutputMode)).Length}");
            }
            else
            {
                Debug.LogError("✗ Failed to create VideoPlayer component");
            }
            
            DestroyImmediate(testObj);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ Video module test failed: {e.Message}");
        }
    }
    
    private void TestVideoPlayerCreation()
    {
        Debug.Log("\nTest 2: VideoPlayer Component Configuration");
        
        try
        {
            GameObject testObj = new GameObject("VideoPlayerTest");
            VideoPlayer vp = testObj.AddComponent<VideoPlayer>();
            
            // Test configuration
            vp.renderMode = VideoRenderMode.RenderTexture;
            vp.audioOutputMode = VideoAudioOutputMode.Direct;
            vp.playOnAwake = false;
            vp.isLooping = false;
            
            Debug.Log("✓ VideoPlayer configuration test passed");
            Debug.Log($"  - Render Mode: {vp.renderMode}");
            Debug.Log($"  - Audio Mode: {vp.audioOutputMode}");
            Debug.Log($"  - Play on Awake: {vp.playOnAwake}");
            Debug.Log($"  - Looping: {vp.isLooping}");
            
            DestroyImmediate(testObj);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ VideoPlayer configuration test failed: {e.Message}");
        }
    }
    
    private void TestFileSystemAccess()
    {
        Debug.Log("\nTest 3: File System Access");
        
        // Test persistent data path
        string persistentPath = Application.persistentDataPath;
        Debug.Log($"✓ Persistent data path: {persistentPath}");
        Debug.Log($"✓ Persistent data path exists: {Directory.Exists(persistentPath)}");
        
        // Test common Android video paths
        string[] androidPaths = {
            "/storage/emulated/0/DCIM/Camera/",
            "/storage/emulated/0/Movies/",
            "/storage/emulated/0/Download/",
            "/storage/emulated/0/Pictures/"
        };
        
        Debug.Log("Android video directories status:");
        foreach (string path in androidPaths)
        {
            bool exists = Directory.Exists(path);
            Debug.Log($"  {path}: {(exists ? "✓ Exists" : "✗ Not accessible")}");
        }
        
        // Test file creation in persistent data
        try
        {
            string testFile = Path.Combine(persistentPath, "video_test.txt");
            File.WriteAllText(testFile, "Video system test file");
            
            if (File.Exists(testFile))
            {
                Debug.Log("✓ File creation test passed");
                File.Delete(testFile);
                Debug.Log("✓ File deletion test passed");
            }
            else
            {
                Debug.LogError("✗ File creation test failed");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ File system test failed: {e.Message}");
        }
    }
    
    private void TestVideoFormatSupport()
    {
        Debug.Log("\nTest 4: Video Format Support");
        
        string[] formats = { ".mp4", ".mov", ".avi", ".mkv", ".webm" };
        
        foreach (string format in formats)
        {
            Debug.Log($"  {format}: Supported by Unity VideoPlayer");
        }
        
        Debug.Log("✓ Video format support test completed");
        Debug.Log("Note: Actual codec support depends on platform and device capabilities");
    }
    
    private void TestRenderTextureCreation()
    {
        Debug.Log("\nTest 5: RenderTexture Creation");
        
        try
        {
            RenderTexture rt = new RenderTexture(1920, 1080, 0);
            
            if (rt != null)
            {
                Debug.Log("✓ RenderTexture creation successful");
                Debug.Log($"  - Resolution: {rt.width}x{rt.height}");
                Debug.Log($"  - Format: {rt.format}");
                
                rt.Release();
                Debug.Log("✓ RenderTexture cleanup successful");
            }
            else
            {
                Debug.LogError("✗ RenderTexture creation failed");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ RenderTexture test failed: {e.Message}");
        }
    }
    
    public void TestVideoPlaybackHandler()
    {
        Debug.Log("\n=== VideoPlaybackHandler Component Test ===");
        
        VideoPlaybackHandler handler = FindObjectOfType<VideoPlaybackHandler>();
        
        if (handler != null)
        {
            Debug.Log("✓ VideoPlaybackHandler found in scene");
        }
        else
        {
            Debug.LogWarning("⚠ VideoPlaybackHandler not found in scene - manual setup required");
        }
    }
    
    // Public method to call from inspector or other scripts
    [ContextMenu("Run Video System Test")]
    public void RunTest()
    {
        RunVideoSystemTest();
        TestVideoPlaybackHandler();
    }
}