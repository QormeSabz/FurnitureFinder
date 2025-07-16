# Video Playback Feature for ChAiR AR Furniture App

## Overview

This feature adds video playback capability to the ChAiR furniture AR app, allowing users to playback videos recorded using Java ARCore or other recording methods. The implementation uses Unity's VideoPlayer component and provides a user-friendly interface for browsing and playing back video files.

## Features

- **Video File Browser**: Automatically scans common video directories on Android devices
- **Video Player Controls**: Play, pause, stop, seek, and volume control
- **Supported Formats**: MP4, MOV, AVI, MKV, WebM
- **File Information**: Display file name and size
- **Integration**: Seamlessly integrates with existing AR interface

## Files Added

### Scripts

1. **VideoPlaybackHandler.cs**
   - Main video playback controller
   - Handles video loading, playback controls, and UI management
   - Manages file browser functionality

2. **VideoFileItem.cs**
   - Helper script for video file list items
   - Handles individual file selection

3. **VideoTestUtility.cs**
   - Development utility for testing video functionality
   - Creates test files and lists available videos

### Modified Files

1. **ObjectHandler.cs**
   - Added video player button and integration
   - Added reference to VideoPlaybackHandler

## Usage Instructions

### For Users

1. **Opening Video Player**
   - In the AR scene, look for the video player button (📹)
   - Tap the button to open the video player interface

2. **Selecting Videos**
   - Tap "Select Video" to open the file browser
   - The app will scan for video files in common directories:
     - `/storage/emulated/0/DCIM/Camera/` (Camera recordings)
     - `/storage/emulated/0/Movies/` (Movie files)
     - `/storage/emulated/0/Download/` (Downloaded videos)
     - App's persistent data directory

3. **Playing Videos**
   - Select a video from the browser list
   - Use the play/pause button to control playback
   - Drag the progress slider to seek to different positions
   - Adjust volume using the volume slider
   - Tap "Stop" to stop playback and return to beginning

4. **Closing Video Player**
   - Tap the "X" button to close the video player
   - The video will stop automatically when closing

### For Developers

#### Setup Requirements

1. **Unity Package Dependencies**
   - `com.unity.modules.video` (already included)
   - `com.unity.ugui` for UI components
   - `com.unity.textmeshpro` for text rendering

2. **GameObject Setup**
   - Add VideoPlaybackHandler component to a GameObject in AR scene
   - Create UI panels for video player and file browser
   - Assign all UI references in the inspector

3. **UI Components Required**
   - Video Player Panel (with RawImage for video display)
   - Control buttons (Play/Pause, Stop, Select Video, Close)
   - Progress and volume sliders
   - Status text displays
   - File Browser Panel with scroll view

#### Integration with ARCore Recordings

The video player can handle videos recorded with Java ARCore by:

1. **File System Access**: Scanning common video directories where ARCore recordings are typically saved
2. **Format Support**: Supporting standard video formats used by ARCore
3. **Path Handling**: Using proper file:// protocol for local file access

#### Code Structure

```
VideoPlaybackHandler
├── Video Player Management
│   ├── VideoPlayer component configuration
│   ├── RenderTexture setup
│   └── Event handling (prepare, start, end, error)
├── UI Management
│   ├── Player controls
│   ├── Progress tracking
│   └── Status updates
├── File Browser
│   ├── Directory scanning
│   ├── File filtering
│   └── Dynamic list creation
└── Integration Points
    ├── ObjectHandler connection
    └── Audio system integration
```

## Technical Implementation Notes

### Video Player Configuration

- **Render Mode**: RenderTexture for UI integration
- **Audio Output**: Direct mode for better performance
- **Playback**: Non-looping by default
- **Threading**: Handles video decoding on separate thread

### File System Scanning

- **Recursive Search**: Scans subdirectories for video files
- **Error Handling**: Graceful handling of permission issues
- **Performance**: Batched scanning with progress updates
- **Filtering**: Extension-based filtering for supported formats

### Memory Management

- **RenderTexture**: Properly released on component destruction
- **Event Cleanup**: Unsubscribed from VideoPlayer events
- **File Handles**: Properly closed after scanning

## Testing

### Development Testing

1. **Test Video Creation**: Use VideoTestUtility to create test files
2. **File Scanning**: Check console logs for discovered video files
3. **Playback Testing**: Verify controls work with test videos

### Device Testing

1. **Record Videos**: Use ARCore app to record test videos
2. **File Discovery**: Verify app finds recorded videos
3. **Playback Quality**: Test performance with various video sizes
4. **Permission Handling**: Test file system access permissions

## Troubleshooting

### Common Issues

1. **No Videos Found**
   - Check file permissions
   - Verify video files are in supported formats
   - Check directory paths are accessible

2. **Video Won't Play**
   - Verify video format is supported
   - Check file path is correct
   - Ensure VideoPlayer component is properly configured

3. **Performance Issues**
   - Lower video resolution if needed
   - Check available memory
   - Verify hardware video decoding support

### Debug Information

Enable debug logging by checking the console for:
- File scanning progress
- Video loading status
- Playback events
- Error messages

## Future Enhancements

Potential improvements for the video playback feature:

1. **Video Recording**: Add ability to record AR sessions directly in the app
2. **Cloud Storage**: Support for videos stored in cloud services
3. **Video Editing**: Basic trimming and editing capabilities
4. **Metadata Display**: Show video duration, resolution, creation date
5. **Thumbnail Generation**: Preview thumbnails for video files
6. **Playlist Support**: Create and manage video playlists
7. **AR Overlay**: Display videos with AR content overlay
8. **Social Sharing**: Share videos directly from the app

## Dependencies

- Unity 2022.3 or later
- AR Foundation 5.1.0
- TextMeshPro 3.0.6
- Unity Video Module 1.0.0
- Android API level 24+ (for optimal video support)

## Platform Support

- **Primary**: Android with ARCore
- **Secondary**: iOS with ARKit (with some modifications needed)
- **Testing**: Unity Editor (limited video format support)