using UnityEngine;
using UnityEngine.Video;
using System.IO;

public class VideoTestUtility : MonoBehaviour
{
    [Header("Test Video Creation")]
    [SerializeField] private string testVideoName = "test_arcore_video.mp4";
    
    public void CreateTestVideoFile()
    {
        // Create a dummy video file for testing purposes
        // In a real scenario, this would be replaced by actual ARCore recorded videos
        string testPath = Path.Combine(Application.persistentDataPath, testVideoName);
        
        if (!File.Exists(testPath))
        {
            // Create an empty file to simulate a video file
            File.WriteAllText(testPath, "This is a test video file created for demonstration purposes.");
            Debug.Log($"Test video file created at: {testPath}");
        }
        else
        {
            Debug.Log($"Test video file already exists at: {testPath}");
        }
    }
    
    public void ListVideoFiles()
    {
        string[] searchPaths = {
            Application.persistentDataPath,
            "/storage/emulated/0/DCIM/Camera/",
            "/storage/emulated/0/Movies/"
        };
        
        Debug.Log("=== Video Files Found ===");
        
        foreach (string path in searchPaths)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path, "*.mp4");
                Debug.Log($"Path: {path}");
                
                if (files.Length == 0)
                {
                    Debug.Log("  No .mp4 files found");
                }
                else
                {
                    foreach (string file in files)
                    {
                        Debug.Log($"  Found: {Path.GetFileName(file)}");
                    }
                }
            }
            else
            {
                Debug.Log($"Path does not exist: {path}");
            }
        }
        
        Debug.Log("=== End of Video Files ===");
    }
    
    public string GetTestVideoPath()
    {
        return Path.Combine(Application.persistentDataPath, testVideoName);
    }
    
    private void Start()
    {
        // Auto-create test file for development
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        CreateTestVideoFile();
        #endif
    }
}