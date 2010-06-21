﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VidCoder.Model;
using System.Windows.Input;
using VidCoder.DragDropUtils;
using HandBrake.Interop;
using System.Diagnostics;

namespace VidCoder.ViewModel
{
    public class EncodeJobViewModel : ViewModelBase, IDragItem
    {
        private MainViewModel mainViewModel;

        private EncodeJob job;
        private bool encoding;
        private int percentComplete;
        private bool isOnlyItem;
        private Stopwatch encodeTimeStopwatch;

        private ICommand removeQueueJobCommand;

        public EncodeJobViewModel(EncodeJob job, MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            this.job = job;
        }

        public EncodeJob Job
        {
            get
            {
                return this.job;
            }
        }

        public EncodingProfile Profile
        {
            get
            {
                return this.Job.EncodingProfile;
            }
        }

        public HandBrakeInstance HandBrakeInstance { get; set; }

        public bool Encoding
        {
            get
            {
                return this.encoding;
            }

            set
            {
                this.encoding = value;
                this.NotifyPropertyChanged("Encoding");
            }
        }

        public bool IsOnlyItem
        {
            get
            {
                return this.isOnlyItem;
            }

            set
            {
                this.isOnlyItem = value;
                this.NotifyPropertyChanged("ShowProgressBar");
            }
        }

        public TimeSpan EncodeTime
        {
            get
            {
                if (this.encodeTimeStopwatch == null)
                {
                    return TimeSpan.Zero;
                }

                return this.encodeTimeStopwatch.Elapsed;
            }
        }

        public bool ShowRemoveButton
        {
            get
            {
                return !this.encoding;
            }
        }

        public bool ShowProgressBar
        {
            get
            {
                return this.encoding && !this.IsOnlyItem;
            }
        }

        public int PercentComplete
        {
            get
            {
                return this.percentComplete;
            }

            set
            {
                this.percentComplete = value;
                this.NotifyPropertyChanged("PercentComplete");
            }
        }

        public string ChaptersDisplay
        {
            get
            {
                if (this.job.ChapterStart == this.job.ChapterEnd)
                {
                    return this.job.ChapterStart.ToString();
                }
                else
                {
                    return this.job.ChapterStart + " - " + this.job.ChapterEnd;
                }
            }
        }

        public string AudioEncodersDisplay
        {
            get
            {
                var encodingParts = new List<string>();
                foreach (AudioEncoding audioEncoding in this.Profile.AudioEncodings)
                {
                    encodingParts.Add(DisplayConversions.DisplayAudioEncoder(audioEncoding.Encoder));
                }

                return string.Join(", ", encodingParts);
            }
        }

        public string VideoQualityDisplay
        {
            get
            {
                switch (this.Profile.VideoEncodeRateType)
                {
                    case VideoEncodeRateType.AverageBitrate:
                        return this.Profile.VideoBitrate + " kbps";
                    case VideoEncodeRateType.TargetSize:
                        return this.Profile.TargetSize + " MB";
                    case VideoEncodeRateType.ConstantQuality:
                        return "CQ " + this.Profile.Quality;
                    default:
                        break;
                }

                return string.Empty;
            }
        }

        public string AudioBitrateDisplay
        {
            get
            {
                var bitrateParts = new List<string>();
                foreach (AudioEncoding audioEncoding in this.Profile.AudioEncodings)
                {
                    if (audioEncoding.Encoder != AudioEncoder.Ac3Passthrough && audioEncoding.Encoder != AudioEncoder.DtsPassthrough)
                    {
                        bitrateParts.Add(audioEncoding.Bitrate.ToString());
                    }
                }

                return string.Join(", ", bitrateParts);
            }
        }

        public ICommand RemoveQueueJobCommand
        {
            get
            {
                if (this.removeQueueJobCommand == null)
                {
                    this.removeQueueJobCommand = new RelayCommand(param =>
                    {
                        this.mainViewModel.RemoveQueueJob(this);
                    },
                    param =>
                    {
                        return !this.Encoding;
                    });
                }

                return this.removeQueueJobCommand;
            }
        }

        public bool CanDrag
        {
            get
            {
                return !this.Encoding;
            }
        }

        public void ReportEncodeStart(bool isOnlyItem)
        {
            this.Encoding = true;
            this.IsOnlyItem = isOnlyItem;
            this.encodeTimeStopwatch = Stopwatch.StartNew();
            this.NotifyPropertyChanged("ShowRemoveButton");
            this.NotifyPropertyChanged("ShowProgressBar");
        }

        public void ReportEncodePause()
        {
            this.encodeTimeStopwatch.Stop();
        }

        public void ReportEncodeResume()
        {
            this.encodeTimeStopwatch.Start();
        }

        public void ReportEncodeEnd()
        {
            this.Encoding = false;
            this.encodeTimeStopwatch.Stop();
            this.NotifyPropertyChanged("ShowRemoveButton");
            this.NotifyPropertyChanged("ShowProgressBar");
        }
    }
}
