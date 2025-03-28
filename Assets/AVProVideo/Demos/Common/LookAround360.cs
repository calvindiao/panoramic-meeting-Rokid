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
		
		// 要播放的媒体URL
		private string mediaUrl = "http://192.168.5.22:18000/hls/Y5jQyJTNg/Y5jQyJTNg_live.m3u8";
		
		
		// 是否在启动时自动播放
		private bool autoPlayOnStart = false;
		
		// 添加ApplyToMesh引用，用于控制材质
		private ApplyToMesh applyToMesh;

		// 添加预加载标志
		private bool isMediaPreloaded = false;

		void Start()
		{
			// 获取ApplyToMesh组件
			//applyToMesh = GetComponent<ApplyToMesh>();
			
			// 设置Android平台选项
			mediaPlayer.PlatformOptionsAndroid.videoApi = Android.VideoApi.ExoPlayer;
			
			// 如果设置了自动播放，则开始播放
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
		
		// 预加载媒体但不播放
		public void PreloadMedia()
		{
			if (mediaPlayer != null)
			{
				// 打开媒体但不自动播放
				mediaPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, mediaUrl, false);
				isMediaPreloaded = true;
				Debug.Log("Media preloaded: " + mediaUrl);
			}
		}
		
		// 开始播放媒体
		public void PlayMedia()
		{
			if (mediaPlayer != null)
			{
				if (!isMediaPreloaded)
				{
					mediaPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, mediaUrl, true);
					isMediaPreloaded = true;
				}
				else
				{
					mediaPlayer.Play();
				}
				
				Debug.Log("Media playback started: " + mediaUrl);
			}
		}
		
		// 恢复已加载的媒体播放
		public void ResumeMedia()
		{
			if (mediaPlayer != null)
			{
				mediaPlayer.Play();
				Debug.Log("Media playback resumed");
			}
		}
		
		// 暂停播放但保持连接
		public void PauseMedia()
		{
			if (mediaPlayer != null)
			{
				mediaPlayer.Pause();
				Debug.Log("Media playback paused");
			}
		}
		
		// 停止播放媒体
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