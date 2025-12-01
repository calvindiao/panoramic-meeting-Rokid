using UnityEngine;
using System.Collections.Generic;

//-----------------------------------------------------------------------------
// Copyright 2015-2021 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo.Demos
{
	/// <summary>
	/// Rotate the transform (usually with Camera attached) to look around during playback of 360/180 videos.
	/// Unity will rotate the camera automatically if VR is enabled, in which case this script does nothing.
	/// Otherwise if there is a gyroscope it will be used, otherwise the mouse/touch can be used.
	/// </summary>
	public class LookAround360 : MonoBehaviour
	{
		public MediaPlayer mediaPlayer;
		
		// URL of the media to play
		private string mediaUrl = "http://192.168.5.20:18000/hls/tqS5rMLNRz/tqS5rMLNRz_live.m3u8";
		private bool autoPlayOnStart = false;
		private ApplyToMesh applyToMesh;
		// Add preload flag
		private bool isMediaPreloaded = false;
		void Start()
		{
			//applyToMesh = GetComponent<ApplyToMesh>();
			mediaPlayer.PlatformOptionsAndroid.videoApi = Android.VideoApi.ExoPlayer;
			
			if (autoPlayOnStart)
			{
				PlayMedia();
			}
			else
			{
				// 默认隐藏或设置材质为黑色
				// if (applyToMesh != null)
				// {
				// 	// 可选：创建并设置黑色材质
				// 	 applyToMesh.enabled = false;
				// }
			}
		}
		
		public void PreloadMedia()
		{
			if (mediaPlayer != null)
			{
				// mediaPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, mediaUrl, false);
				// isMediaPreloaded = true;
				Debug.Log("Calvin_Not Execute!!! Media preloaded: " + mediaUrl);
			}
		}
		
		public void PlayMedia()
		{
			if (mediaPlayer != null)
			{
				if (!isMediaPreloaded)
				{
					mediaPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, mediaUrl, true);
					//isMediaPreloaded = true;
				}
				else
				{
					mediaPlayer.Play();
				}
				
				Debug.Log("Media playback started: " + mediaUrl);
			}
		}
		public void ResumeMedia()
		{
			if (mediaPlayer != null)
			{
				mediaPlayer.Play();
				Debug.Log("Media playback resumed");
			}
		}
		public void PauseMedia()
		{
			if (mediaPlayer != null)
			{
				mediaPlayer.Pause();
				Debug.Log("Media playback paused");
			}
		}
		public void StopMedia()
		{
			if (mediaPlayer != null)
			{
				mediaPlayer.CloseMedia();
				isMediaPreloaded = false;
				
				// 禁用ApplyToMesh组件
				// if (applyToMesh != null)
				// {
				// 	applyToMesh.enabled = false;
				// }
				
				Debug.Log("Media playback stopped");
			}
		}

		public void SetUrl(string url)
		{
			mediaUrl = url;
			Debug.Log("Calvin_SetMediaUrl: " + mediaUrl);
		}

		void Update()
		{

		}

		void OnDestroy()
		{
			// 确保在销毁时关闭媒体
			if (mediaPlayer != null)
			{
				mediaPlayer.CloseMedia();
			}
		}
	}
}