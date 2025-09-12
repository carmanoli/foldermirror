using FolderMirror;
using System;
using System.Collections.Generic;


namespace FolderMirror
{

    public class FileMirror
    {
        Logger _logger;

        public FileMirror(Logger logger)
        {
            _logger = logger;
        }

        public bool startFolderMirror(string rootSourceFolder, string rootTargetFolder)
        {
            var sourceFolders = Directory.GetDirectories(rootSourceFolder).Select(Path.GetFileName).ToHashSet();
            var targetFolders = Directory.GetDirectories(rootTargetFolder).Select(Path.GetFileName).ToHashSet();

            startFileMirror(rootSourceFolder, rootTargetFolder);

            // Delete folders that exist on the target and not on the source
            foreach (var deletedFolder in targetFolders.Except(sourceFolders))
            {
                var targetDeletedFolder = Path.Combine(rootTargetFolder, deletedFolder);


       
                try
                {
                    ClearReadOnly(new DirectoryInfo(targetDeletedFolder)); // clear attribute or it won't delete
                    Directory.Delete(targetDeletedFolder, true); // delete recursively
                    _logger.Log($"Deleted folder: {targetDeletedFolder}");
                }
                catch (Exception ex)
                {
                    _logger.Log($"Error deleting folder: {targetDeletedFolder}: {ex.Message}");
                }
            }

            // Create folders that exist in the source and not on the target, recursivelly
            foreach (var newFolder in sourceFolders.Except(targetFolders))
            {

                var newSourceFolder = Path.Combine(rootSourceFolder, newFolder);
                var newTargetFolder = Path.Combine(rootTargetFolder, newFolder);
                try
                {
                    Directory.CreateDirectory(newTargetFolder);
                    _logger.Log($"Created folder: {newTargetFolder}");
                    startFolderMirror(newSourceFolder, newTargetFolder);
                }
                catch (Exception ex)
                {
                    _logger.Log($"Error creating folder: {newTargetFolder}: {ex.Message}");
                }

                // set attributes of folders
                var sourceFolderInfo = new DirectoryInfo(newSourceFolder);
                var targetFolderInfo = new DirectoryInfo(newTargetFolder);
                targetFolderInfo.Attributes = sourceFolderInfo.Attributes;
            }

            // Recursivelly, dig up folders that exists in both source and target
            foreach (var sameFolder in sourceFolders.Intersect(targetFolders))
            {
                var sameSourceFolder = Path.Combine(rootSourceFolder, sameFolder);
                var sameTargetFolder = Path.Combine(rootTargetFolder, sameFolder);

                startFolderMirror(sameSourceFolder, sameTargetFolder);

                // set attributes of folders
                var sourceFolderInfo = new DirectoryInfo(sameSourceFolder);
                var targetFolderInfo = new DirectoryInfo(sameTargetFolder);
                if (targetFolderInfo.Attributes != sourceFolderInfo.Attributes)
                {
                    targetFolderInfo.Attributes = sourceFolderInfo.Attributes;
                }
            }

            return true;
        }

        public bool startFileMirror(string rootSourceFolder, string rootTargetFolder)
        {
            var sourceFiles = Directory.GetFiles(rootSourceFolder).Select(Path.GetFileName).ToHashSet();
            var targetFiles = Directory.GetFiles(rootTargetFolder).Select(Path.GetFileName).ToHashSet();

            // Delete files that exist on the target and not on the source
            foreach (var deletedFile in targetFiles.Except(sourceFiles))
            {
                var targetDeletedFile = Path.Combine(rootTargetFolder, deletedFile);

                var targetDeletedFileInfo = new FileInfo(targetDeletedFile);


                try
                {
                    if (targetDeletedFileInfo.IsReadOnly)
                    {
                        targetDeletedFileInfo.IsReadOnly = false; 
                    }
                    File.Delete(targetDeletedFile);
                    _logger.Log($"Deleted file: {targetDeletedFile}");
                }
                catch (Exception ex)
                {
                    _logger.Log($"Error deleting file: {targetDeletedFile}: {ex.Message}");
                }

            }

            // Create files that exist in the source and not on the target
            foreach (var newFile in sourceFiles.Except(targetFiles))
            {
                var newSourceFile = Path.Combine(rootSourceFolder, newFile);
                var newTargetFile = Path.Combine(rootTargetFolder, newFile);
                try
                {

                    File.Copy(newSourceFile, newTargetFile);
                    _logger.Log($"Copied file: {newTargetFile}");
                }
                catch (Exception ex)
                {
                    _logger.Log($"Error copying file: {newTargetFile}: {ex.Message}");
                }
                // set attributes of folders
                var sourceFileInfo = new FileInfo(newSourceFile);
                var targetFileInfo = new FileInfo(newTargetFile);
                targetFileInfo.Attributes = sourceFileInfo.Attributes;
            }

            // If a file exists in both source and target, we will have to compare it
            foreach (var sameFile in sourceFiles.Intersect(targetFiles))
            {
                var sourceFile = Path.Combine(rootSourceFolder, sameFile);
                var targetFile = Path.Combine(rootTargetFolder, sameFile);

                if (isFileDifferent(sourceFile, targetFile)){
                    try
                    {
                        File.Copy(sourceFile, targetFile, true); // overwrite
                        _logger.Log($"Copied file: {targetFile}");
                    }
                    catch (Exception ex)
                    {
                        _logger.Log($"Error copying file: {targetFile}: {ex.Message}");
                    }
                }
                var sourceFileInfo = new FileInfo(sourceFile);
                var targetFileInfo = new FileInfo(targetFile);
                if (targetFileInfo.Attributes != sourceFileInfo.Attributes)
                {
                    // set attributes of files
                    targetFileInfo.Attributes = sourceFileInfo.Attributes;
                }
            }

            return true;
        }

        private bool isFileDifferent(string sourceFile, string targetFile)
        {
            // Compare file size, timestamp and MD5 hash
            FileInfo sourceInfo = new FileInfo(sourceFile);
            FileInfo targetInfo = new FileInfo(targetFile);

            if (sourceInfo.Length != targetInfo.Length)
            {
                _logger.Log($"Files have different Length, source: {sourceFile}, target: {targetFile}");
                return true;
            }
            if (sourceInfo.LastWriteTimeUtc != targetInfo.LastWriteTimeUtc)
            {
                _logger.Log($"Files have different timestamp, source: {sourceFile}, target: {targetFile}");
                return true;
            }
            if (!getMD5HASH(sourceFile).SequenceEqual(getMD5HASH(targetFile)))
            {
                _logger.Log($"Files have different hash, source: {sourceFile}, target: {targetFile}");
                return true;
            }
            return false;
        }

        private byte[] getMD5HASH(string filePath)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create()) {
                using (var stream = File.OpenRead(filePath)) {
                    return md5.ComputeHash(stream);
                }
            }
        }

        private void ClearReadOnly(DirectoryInfo rootTargetFolder)
        {
            rootTargetFolder.Attributes &= ~FileAttributes.ReadOnly;
            foreach (FileInfo targetFile in rootTargetFolder.GetFiles())
                {
                    targetFile.IsReadOnly = false;
                }
                foreach (DirectoryInfo targetFolder in rootTargetFolder.GetDirectories())
                {
                ClearReadOnly(targetFolder);
                }
            
        }
    }
}
