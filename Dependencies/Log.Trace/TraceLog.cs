﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D;
using Core2D.Interfaces;
using System;
using System.Diagnostics;

namespace Dependencies
{
    /// <summary>
    /// Trace message logger.
    /// </summary>
    public class TraceLog : ObservableObject, ILog
    {
        private const string InformationPrefix = "Information: ";
        private const string WarningPrefix = "Warning: ";
        private const string ErrorPrefix = "Error: ";

        private string _lastMessage;

        /// <inheritdoc/>
        public string LastMessage
        {
            get { return _lastMessage; }
            set { Update(ref _lastMessage, value); }
        }

        /// <inheritdoc/>
        public void Initialize(string path)
        {
            try
            {
                Trace.Listeners.Add(new TextWriterTraceListener(path, "listener"));
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        /// <inheritdoc/>
        public void Close()
        {
            try
            {
                Trace.Flush();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        /// <inheritdoc/>
        public void LogInformation(string message)
        {
            Trace.TraceInformation(message);
            LastMessage = InformationPrefix + message;
        }

        /// <inheritdoc/>
        public void LogInformation(string format, params object[] args)
        {
            Trace.TraceInformation(format, args);
            LastMessage = InformationPrefix + string.Format(format, args);
        }

        /// <inheritdoc/>
        public void LogWarning(string message)
        {
            Trace.TraceWarning(message);
            LastMessage = WarningPrefix + message;
        }

        /// <inheritdoc/>
        public void LogWarning(string format, params object[] args)
        {
            Trace.TraceWarning(format, args);
            LastMessage = WarningPrefix + string.Format(format, args);
        }

        /// <inheritdoc/>
        public void LogError(string message)
        {
            Trace.TraceError(message);
            LastMessage = ErrorPrefix + message;
        }

        /// <inheritdoc/>
        public void LogError(string format, params object[] args)
        {
            Trace.TraceError(format, args);
            LastMessage = ErrorPrefix + string.Format(format, args);
        }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        ~TraceLog()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose unmanaged resources.
        /// </summary>
        /// <param name="disposing">The flag indicating whether disposing.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }
    }
}
