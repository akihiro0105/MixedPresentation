using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR || UNITY_UWP
#if !UNITY_2017_2_OR_NEWER

#else

#endif
#endif

namespace HoloLensModule.Utility
{
    public class FileIOControl
    {
        // Save Load
        // テキスト(string), zip, 画像, 音声, 動画
        // zip作成,解凍
        // UWP : local, document, picture ohter
        // other : streamingassets, 好きなところ

        [Serializable]
        public enum FileIOState
        {
            Success,
            Failed,
            PermissionError,
            NotFoundError
        }

        [Serializable]
        public enum DataType
        {
            Folder,
            Text,
            Sound,
            Image,
            Video,
            Other
        }

        [Serializable]
        public enum RootFolder
        {
            // UWP
            LocalFolder,
            DocumentFolder,
            PictureFolder,
            // Editor Standalone
            StreamingAssetsFolder,
            Other
        }

        public FileIOControl()
        {

        }

        public void CreateFolder(RootFolder rootfolder,string foldername,Action<FileIOState> function)
        {
            function.Invoke(FileIOState.Success);
        }

        public void SaveStringData(RootFolder rootfolder,string data,string filename,Action<FileIOState> function)
        {
            function.Invoke(FileIOState.Success);
        }

        public void LoadStringData(RootFolder rootfolder, string filename,Action<string> function)
        {
            string data = "";
            function.Invoke(data);
        }

        public void CreateZip(RootFolder rootfolder,string filename)
        {

        }

        public void MoveFile()
        {

        }

        public void CopyFile()
        {

        }

        public void DeleteFile()
        {

        }

        public void DeleteFolder()
        {

        }

        public List<string> ViewDataList()
        {
            List<string> list = new List<string>();
            return list;
        }
    }
}
