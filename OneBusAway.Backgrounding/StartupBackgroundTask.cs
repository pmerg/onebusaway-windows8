﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace OneBusAway.Backgrounding
{
    /// <summary>
    /// This task should start when the user logs in.
    /// </summary>
    public sealed class StartupBackgroundTask : IBackgroundTask
    {
        /// <summary>
        /// Runs the tile updater service.
        /// </summary>
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            taskInstance.Canceled += OnTaskInstanceCanceled;
            TileUpdaterService.Instance.CreateIfNeccessary(taskInstance.GetDeferral());
        }

        /// <summary>
        /// Cancels the task.
        /// </summary>
        private void OnTaskInstanceCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            TileUpdaterService.Instance.Abort();
        }
    }
}