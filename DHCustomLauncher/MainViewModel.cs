using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHCustomLauncher
{
    class MainViewModel : INotifyPropertyChanged
    {
        //public String BindText { get; set; } = "初期値";

        public int _resolution = -1;
        public bool _isCustomResolution = false;
        public bool _isDebugLog = false;
        public int Resolution
        {
            set
            {
                if (value == 6) //Custom
                {
                    this._isCustomResolution = true;
                    this.OnPropertyChanged(nameof(IsCustomResolution));
                }
                else
                {
                    this._isCustomResolution = false;
                    this.OnPropertyChanged(nameof(IsCustomResolution));
                }
            }
        }

        public bool IsCustomResolution
        {
            get
            {
                return this._isCustomResolution;
            }
            set
            {
                //this._isCustomResolution = value;
                //this.OnPropertyChanged(nameof(IsCustomResolution));

                return;
            }
        }

        public bool IsDebugLog
        {
            get
            {
                return this._isDebugLog;
            }
            set
            {
                this._isDebugLog = value;
                this.OnPropertyChanged(nameof(IsDebugLog));

                return;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = null;
        protected void OnPropertyChanged(string info)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

    }
}
