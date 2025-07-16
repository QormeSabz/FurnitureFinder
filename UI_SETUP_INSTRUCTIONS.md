# Video Player UI Setup Instructions

## Required UI Structure

To implement the video playback feature, the following UI components need to be added to the AR scene:

### Main Video Player Panel
```
VideoPlayerPanel (Panel)
├── VideoDisplay (RawImage) - for video content
├── ControlsPanel (Panel)
│   ├── PlayPauseButton (Button + Image)
│   ├── StopButton (Button)
│   ├── SelectVideoButton (Button)
│   └── ClosePlayerButton (Button)
├── ProgressPanel (Panel)
│   ├── ProgressSlider (Slider)
│   └── TimeText (TextMeshPro)
├── VolumePanel (Panel)
│   ├── VolumeSlider (Slider)
│   └── VolumeIcon (Image)
└── StatusText (TextMeshPro)
```

### File Browser Panel
```
FileBrowserPanel (Panel)
├── HeaderPanel (Panel)
│   ├── BrowserTitle (TextMeshPro) - "Select Video File"
│   └── CloseBrowserButton (Button)
├── ContentPanel (ScrollRect)
│   └── Content (Content Size Fitter + Vertical Layout Group)
│       └── [Dynamic VideoFileItem instances]
└── StatusText (TextMeshPro)
```

### Video File Item Prefab
```
VideoFileItem (Panel + Button)
├── BackgroundImage (Image)
├── FileNameText (TextMeshPro)
└── SelectButton (Button) - invisible, covers entire item
```

## Setup Instructions

1. **Create Video Player Panel**
   - Add as child of main Canvas
   - Set initial state to inactive
   - Position to cover most of screen

2. **Add VideoPlaybackHandler Component**
   - Attach to GameObject in scene (can be on Canvas)
   - Assign all UI references in inspector

3. **Create File Browser Panel**
   - Add as child of main Canvas
   - Set initial state to inactive
   - Use ScrollRect for scrollable file list

4. **Update ObjectHandler**
   - Add reference to video player button in UI
   - Assign VideoPlaybackHandler reference

5. **Create Video File Item Prefab**
   - Save as prefab in Assets/Prefabs/
   - Assign to VideoPlaybackHandler's videoFileItemPrefab field

## UI Positioning

- **Video Player Panel**: Center screen, 80% width/height
- **File Browser Panel**: Center screen, 70% width/height
- **Video Player Button**: Add to main AR UI, positioned with other tool buttons
- **Controls**: Bottom of video display
- **Progress**: Below controls
- **Volume**: Side of controls

## Required Assets

### Icons Needed
- Play icon (triangle)
- Pause icon (two rectangles)
- Stop icon (square)
- Video icon (camera/film)
- Volume icon (speaker)
- Close icon (X)

### Sprites
Create simple geometric sprites or use Unity's default UI sprites:
- UISprite for backgrounds
- Knob for sliders
- Checkmark for buttons

## Component References

In VideoPlaybackHandler inspector, assign:
- videoPlayerPanel: Main video panel GameObject
- videoDisplay: RawImage component for video
- All button components
- All slider components
- All text components
- fileBrowserPanel: File browser GameObject
- fileBrowserContent: Content transform of ScrollRect
- videoFileItemPrefab: Prefab for file list items

## Testing Setup

1. Build and deploy to Android device
2. Record some videos using ARCore or camera app
3. Open ChAiR app and navigate to AR scene
4. Tap video player button
5. Select a video file and test playback

## Troubleshooting UI Setup

- Ensure all UI components are properly parented
- Check that panels are initially set to inactive
- Verify all inspector references are assigned
- Test UI scaling on different screen sizes
- Check that buttons have proper interaction zones