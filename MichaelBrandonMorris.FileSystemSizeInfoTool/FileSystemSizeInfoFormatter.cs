﻿using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using MichaelBrandonMorris.Extensions.OtherExtensions;

namespace MichaelBrandonMorris.FileSystemSizeInfoTool
{
    internal class FileSystemSizeInfoFormatter
    {
        private const string FileContentsHeader = "# Files";
        private const string FileExtensionHeader = "File Extension";
        private const string FolderContentsHeader = "# Folders";
        private const string LevelHeader = "Level";
        private const int MaxResults = 1048574;
        private const string OwnerHeader = "Owner";
        private const string PathHeader = "Path";
        private const string ResultNumberHeader = "#";
        private const string SizeHeader = "Size (MB)";

        internal FileSystemSizeInfoFormatter(
            CancellationToken cancellationToken,
            IEnumerable<FileSystemSizeInfo> fileSystemSizeInfos,
            bool shouldSplitPaths,
            int? maxPathLevels)
        {
            CancellationToken = cancellationToken;
            FileSystemSizeInfos = fileSystemSizeInfos;
            ShouldSplitPaths = shouldSplitPaths;
            MaxPathLevels = maxPathLevels;
        }

        private CancellationToken CancellationToken
        {
            get;
        }

        private IEnumerable<FileSystemSizeInfo> FileSystemSizeInfos
        {
            get;
        }

        private int? MaxPathLevels
        {
            get;
        }

        private bool ShouldSplitPaths
        {
            get;
        }

        internal string GetFormattedFileSystemSizeInfos()
        {
            var formattedFileSystemSizeInfos = new StringBuilder();
            var header = new StringBuilder();
            header.AppendCsv(ResultNumberHeader);

            if (ShouldSplitPaths && MaxPathLevels != null)
            {
                for (var i = 0; i < MaxPathLevels; i++)
                {
                    CancellationToken.ThrowIfCancellationRequested();
                    header.AppendCsv($"{LevelHeader} {i}");
                }
            }
            else
            {
                header.AppendCsv(PathHeader);
            }

            header.AppendCsv(FileExtensionHeader);
            header.AppendCsv(FolderContentsHeader);
            header.AppendCsv(FileContentsHeader);
            header.AppendCsv(SizeHeader);
            header.AppendCsv(OwnerHeader);

            formattedFileSystemSizeInfos.AppendLine(header.ToString());

            var resultsCount = 0;

            foreach (var fileSystemSizeInfo in FileSystemSizeInfos)
            {
                CancellationToken.ThrowIfCancellationRequested();

                if (resultsCount == MaxResults)
                {
                    break;
                }

                formattedFileSystemSizeInfos.AppendCsv(
                    resultsCount.ToString());

                if (ShouldSplitPaths && MaxPathLevels != null)
                {
                    for (var i = 0; i < MaxPathLevels.Value; i++)
                    {
                        CancellationToken.ThrowIfCancellationRequested();

                        formattedFileSystemSizeInfos.AppendCsv(
                            i < fileSystemSizeInfo.PathLevels
                                ? fileSystemSizeInfo.FullNameSplitPath[i]
                                : string.Empty);
                    }
                }
                else
                {
                    formattedFileSystemSizeInfos.AppendCsv(
                        fileSystemSizeInfo.FullName);
                }

                formattedFileSystemSizeInfos.AppendCsv(
                    fileSystemSizeInfo.Extension);

                formattedFileSystemSizeInfos.AppendCsv(
                    fileSystemSizeInfo.FoldersCount.ToString());

                formattedFileSystemSizeInfos.AppendCsv(
                    fileSystemSizeInfo.FilesCount.ToString());

                formattedFileSystemSizeInfos.AppendCsv(
                    fileSystemSizeInfo.Size.ToString(
                        CultureInfo.InvariantCulture));

                formattedFileSystemSizeInfos.AppendCsv(
                    fileSystemSizeInfo.Owner);

                formattedFileSystemSizeInfos.AppendLine();
                resultsCount++;
            }

            return formattedFileSystemSizeInfos.ToString();
        }
    }
}