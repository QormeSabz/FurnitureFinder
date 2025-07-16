using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

[System.Serializable]
public class VideoPlayerUISetup : MonoBehaviour
{
    [Header("Auto-Setup Configuration")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private Canvas targetCanvas;
    
    [Header("UI Prefab Resources")]
    [SerializeField] private GameObject videoPlayerPanelPrefab;
    [SerializeField] private GameObject fileBrowserPanelPrefab;
    [SerializeField] private GameObject videoFileItemPrefab;
    
    [Header("Generated Components")]
    [SerializeField] private VideoPlaybackHandler videoPlaybackHandler;
    [SerializeField] private GameObject videoPlayerPanel;
    [SerializeField] private GameObject fileBrowserPanel;
    
    private void Start()
    {
        if (autoSetupOnStart && targetCanvas != null)
        {
            SetupVideoPlayerUI();
        }
    }
    
    [ContextMenu("Setup Video Player UI")]
    public void SetupVideoPlayerUI()
    {
        Debug.Log("Setting up Video Player UI...");
        
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
            if (targetCanvas == null)
            {
                Debug.LogError("No Canvas found in scene!");
                return;
            }
        }
        
        // Create or find VideoPlaybackHandler
        if (videoPlaybackHandler == null)
        {
            videoPlaybackHandler = FindObjectOfType<VideoPlaybackHandler>();
            if (videoPlaybackHandler == null)
            {
                GameObject handlerObj = new GameObject("VideoPlaybackHandler");
                handlerObj.transform.SetParent(targetCanvas.transform);
                videoPlaybackHandler = handlerObj.AddComponent<VideoPlaybackHandler>();
                Debug.Log("✓ Created VideoPlaybackHandler component");
            }
        }
        
        // Setup Video Player Panel
        SetupVideoPlayerPanel();
        
        // Setup File Browser Panel
        SetupFileBrowserPanel();
        
        // Setup Video Player Button in main UI
        SetupVideoPlayerButton();
        
        // Connect everything
        ConnectUIComponents();
        
        Debug.Log("Video Player UI setup completed!");
    }
    
    private void SetupVideoPlayerPanel()
    {
        if (videoPlayerPanel != null) return;
        
        // Create main video player panel
        videoPlayerPanel = new GameObject("VideoPlayerPanel");
        videoPlayerPanel.transform.SetParent(targetCanvas.transform, false);
        
        // Add and configure RectTransform
        RectTransform panelRect = videoPlayerPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = new Vector2(50, 50);
        panelRect.offsetMax = new Vector2(-50, -50);
        
        // Add background
        Image panelBg = videoPlayerPanel.AddComponent<Image>();
        panelBg.color = new Color(0, 0, 0, 0.8f);
        
        // Create video display
        GameObject videoDisplay = new GameObject("VideoDisplay");
        videoDisplay.transform.SetParent(videoPlayerPanel.transform, false);
        
        RectTransform displayRect = videoDisplay.AddComponent<RectTransform>();
        displayRect.anchorMin = new Vector2(0.1f, 0.3f);
        displayRect.anchorMax = new Vector2(0.9f, 0.9f);
        displayRect.offsetMin = Vector2.zero;
        displayRect.offsetMax = Vector2.zero;
        
        RawImage rawImage = videoDisplay.AddComponent<RawImage>();
        rawImage.color = Color.black;
        
        // Create controls panel
        SetupControlsPanel(videoPlayerPanel);
        
        videoPlayerPanel.SetActive(false);
        Debug.Log("✓ Created Video Player Panel");
    }
    
    private void SetupControlsPanel(GameObject parent)
    {
        GameObject controlsPanel = new GameObject("ControlsPanel");
        controlsPanel.transform.SetParent(parent.transform, false);
        
        RectTransform controlsRect = controlsPanel.AddComponent<RectTransform>();
        controlsRect.anchorMin = new Vector2(0.1f, 0.05f);
        controlsRect.anchorMax = new Vector2(0.9f, 0.25f);
        controlsRect.offsetMin = Vector2.zero;
        controlsRect.offsetMax = Vector2.zero;
        
        // Add horizontal layout group
        HorizontalLayoutGroup layoutGroup = controlsPanel.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = 10;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        
        // Create buttons
        CreateButton(controlsPanel, "PlayPauseButton", "▶");
        CreateButton(controlsPanel, "StopButton", "■");
        CreateButton(controlsPanel, "SelectVideoButton", "📁");
        CreateButton(controlsPanel, "CloseButton", "✕");
        
        // Create progress slider
        CreateSlider(controlsPanel, "ProgressSlider", 0f, 100f, 0f);
        
        Debug.Log("✓ Created Controls Panel");
    }
    
    private GameObject CreateButton(GameObject parent, string name, string text)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent.transform, false);
        
        // Add Button component
        Button button = buttonObj.AddComponent<Button>();
        
        // Add Image component for background
        Image bgImage = buttonObj.AddComponent<Image>();
        bgImage.color = Color.white;
        
        // Add text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        TextMeshProUGUI textComp = textObj.AddComponent<TextMeshProUGUI>();
        textComp.text = text;
        textComp.fontSize = 24;
        textComp.color = Color.black;
        textComp.alignment = TextAlignmentOptions.Center;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Set button size
        LayoutElement layoutElement = buttonObj.AddComponent<LayoutElement>();
        layoutElement.minWidth = 60;
        layoutElement.minHeight = 40;
        
        return buttonObj;
    }
    
    private GameObject CreateSlider(GameObject parent, string name, float min, float max, float value)
    {
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent.transform, false);
        
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = value;
        
        // Create background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderObj.transform, false);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = Color.gray;
        
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        // Create handle
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(sliderObj.transform, false);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        
        slider.handleRect = handle.GetComponent<RectTransform>();
        slider.targetGraphic = handleImage;
        
        // Set layout
        LayoutElement layoutElement = sliderObj.AddComponent<LayoutElement>();
        layoutElement.flexibleWidth = 1;
        layoutElement.minHeight = 30;
        
        return sliderObj;
    }
    
    private void SetupFileBrowserPanel()
    {
        if (fileBrowserPanel != null) return;
        
        fileBrowserPanel = new GameObject("FileBrowserPanel");
        fileBrowserPanel.transform.SetParent(targetCanvas.transform, false);
        
        RectTransform panelRect = fileBrowserPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.15f, 0.15f);
        panelRect.anchorMax = new Vector2(0.85f, 0.85f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        Image panelBg = fileBrowserPanel.AddComponent<Image>();
        panelBg.color = new Color(1, 1, 1, 0.9f);
        
        // Create header
        GameObject header = new GameObject("Header");
        header.transform.SetParent(fileBrowserPanel.transform, false);
        
        RectTransform headerRect = header.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 0.9f);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.offsetMin = Vector2.zero;
        headerRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI headerText = header.AddComponent<TextMeshProUGUI>();
        headerText.text = "Select Video File";
        headerText.fontSize = 24;
        headerText.color = Color.black;
        headerText.alignment = TextAlignmentOptions.Center;
        
        // Create close button
        CreateButton(header, "CloseBrowserButton", "✕");
        
        // Create scroll view
        CreateScrollView(fileBrowserPanel);
        
        fileBrowserPanel.SetActive(false);
        Debug.Log("✓ Created File Browser Panel");
    }
    
    private void CreateScrollView(GameObject parent)
    {
        GameObject scrollView = new GameObject("ScrollView");
        scrollView.transform.SetParent(parent.transform, false);
        
        RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 0.1f);
        scrollRect.anchorMax = new Vector2(1, 0.85f);
        scrollRect.offsetMin = Vector2.zero;
        scrollRect.offsetMax = Vector2.zero;
        
        ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
        
        // Create content
        GameObject content = new GameObject("Content");
        content.transform.SetParent(scrollView.transform, false);
        
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        
        VerticalLayoutGroup layoutGroup = content.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 5;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        
        ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        scroll.content = contentRect;
        scroll.horizontal = false;
        scroll.vertical = true;
    }
    
    private void SetupVideoPlayerButton()
    {
        // Try to find ObjectHandler and add button reference
        ObjectHandler objectHandler = FindObjectOfType<ObjectHandler>();
        if (objectHandler != null)
        {
            Debug.Log("✓ Found ObjectHandler - manual button assignment needed");
        }
        else
        {
            Debug.LogWarning("⚠ ObjectHandler not found - manual setup required");
        }
    }
    
    private void ConnectUIComponents()
    {
        if (videoPlaybackHandler == null) return;
        
        // This would need to be done manually in the inspector
        // as we can't directly assign private serialized fields via script
        
        Debug.Log("⚠ UI component assignment needs to be done manually in inspector:");
        Debug.Log("  1. Assign videoPlayerPanel to VideoPlaybackHandler");
        Debug.Log("  2. Assign fileBrowserPanel to VideoPlaybackHandler");
        Debug.Log("  3. Assign all UI control references");
        Debug.Log("  4. Assign video player button in ObjectHandler");
    }
    
    [ContextMenu("Cleanup Video UI")]
    public void CleanupVideoUI()
    {
        if (videoPlayerPanel != null)
        {
            DestroyImmediate(videoPlayerPanel);
            videoPlayerPanel = null;
        }
        
        if (fileBrowserPanel != null)
        {
            DestroyImmediate(fileBrowserPanel);
            fileBrowserPanel = null;
        }
        
        Debug.Log("Video UI cleaned up");
    }
}