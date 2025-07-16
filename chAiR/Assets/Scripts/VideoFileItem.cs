using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoFileItem : MonoBehaviour
{
    [SerializeField] private TMP_Text fileNameText;
    [SerializeField] private Button selectButton;
    [SerializeField] private Image backgroundImage;
    
    private string filePath;
    
    public void Setup(string path, System.Action<string> onSelectCallback)
    {
        filePath = path;
        
        if (fileNameText != null)
        {
            string fileName = System.IO.Path.GetFileName(path);
            fileNameText.text = fileName;
        }
        
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelectCallback?.Invoke(filePath));
        }
    }
    
    public void SetHighlight(bool highlighted)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = highlighted ? Color.cyan : Color.white;
        }
    }
}