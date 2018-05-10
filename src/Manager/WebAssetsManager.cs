﻿using System;
using System.IO;
using System.Net;
using System.IO.Compression;
using System.Diagnostics;

namespace QuickView.Manager
{
    public class WebAssetsManager
    {
        private String tempDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");

        private String projectResourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");

        /// <summary>  
        /// Update resources (js, css, html) to latest version
        /// </summary>  
        /// <param name="link">URL to download resources from</param>
        public void Update(String link)
        {
            try
            {
                Directory.CreateDirectory(tempDirectory);

                // Project resources directory
                //String projectResourcesPath = "src/resources";


                // Download zip file
                Debug.WriteLine("DOWNLOADING FILES...");
                String zipFilePath = Download(link);
                Debug.WriteLine("OK!");
                Debug.WriteLine("");

                // Extract files from zip archive
                Debug.WriteLine("EXTRACTING FILES...");
                String unzipedPath = Unzip(zipFilePath);
                Debug.WriteLine("OK!");
                Debug.WriteLine("");

                // Clean project's resources directory
                Debug.WriteLine("CLEANING RESOURCE DIRECTORY...");
                //clean(projectResourcesPath);
                Clean(null);
                Debug.WriteLine("OK!");
                Debug.WriteLine("");

                // Copy downloaded resources to project directory
                Debug.WriteLine("COPYING FILES...");
                // Copy files to project directory (as backup)
                // copy(inResourcesPath);
                // Copy files to target directory to get latest changes without an application restart
                Copy(unzipedPath);
                Debug.WriteLine("OK!");
                Debug.WriteLine("");

                // Remove temp directory
                Debug.WriteLine("REMOVING TEMP DIRECTORY...");
                Clean(tempDirectory);
                Debug.WriteLine("OK!");
                Debug.WriteLine("");
            }
            catch (Exception)
            {
                Clean(tempDirectory);
                Debug.WriteLine("THERE IS NO INTERNET CONNECTION");
                Debug.WriteLine("BUILT IN RESOURCES WILL BE USED");
            }
        }

        /// <summary>
        /// Download the resources
        /// </summary>
        /// <param name="link">Resources URL</param>   
        /// <returns>Path of the downloaded zip file</returns>
        private String Download(String link)
        {
            try
            {
                String zipFilePath = Path.Combine(tempDirectory, "temp.zip");
                // Download file
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile(new System.Uri(link), zipFilePath);
                }
                return zipFilePath;
            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Unzip the package
        /// </summary>
        /// <param name="zipFilePath">Path for the zip package</param>
        /// <returns>Path of the unziped files</returns>
        private String Unzip(String zipFilePath)
        {
            try
            {
                String unzipedPath = Path.Combine(tempDirectory, "unziped");
                Directory.CreateDirectory(unzipedPath);
                using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.Contains("resources"))
                        {
                            if (!String.IsNullOrEmpty(entry.Name)) {
                                string[] pathParts = Path.GetDirectoryName(entry.FullName).Split('\\');
                                String currentDirectory = Path.Combine(unzipedPath, pathParts[pathParts.Length - 1]);
                                if (!Directory.Exists(currentDirectory))
                                {
                                    Directory.CreateDirectory(currentDirectory);
                                }
                                entry.ExtractToFile(Path.Combine(currentDirectory, entry.Name), true);
                            }  
                        }
                    }
                }
                return unzipedPath;
            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Remove all built in resources
        /// </summary>
        /// <param name="path">Path of the folder to be cleaned</param>
        private void Clean(String path)
        {
            if (String.IsNullOrEmpty(path))
            {
                DirectoryInfo resources = new DirectoryInfo(projectResourcesPath);

                foreach (DirectoryInfo directory in resources.GetDirectories())
                {
                    Directory.Delete(directory.FullName, true);
                }
            } else {
                Directory.Delete(path, true);
            }
        }

        private void Copy(String inResourcesPath)
        {
            try
            {
                string[] directories = Directory.GetDirectories(inResourcesPath);

                // Copy the files and overwrite destination files if they already exist.
                foreach (string directory in directories)
                {
                    string[] pathParts = directory.Split('\\');
                    String currentDirectory = Path.Combine(projectResourcesPath, pathParts[pathParts.Length - 1]);
                    Directory.Move(directory, currentDirectory);
                }
            } catch(Exception ex) {
                throw ex;
            }
        }
    }
}