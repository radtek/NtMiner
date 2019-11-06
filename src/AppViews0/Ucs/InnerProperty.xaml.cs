﻿using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class InnerProperty : UserControl {
        public InnerPropertyViewModel Vm {
            get {
                return (InnerPropertyViewModel)this.DataContext;
            }
        }

        public InnerProperty() {
            InitializeComponent();
            this.RunOneceOnLoaded(window => {
                window.WindowContextEventPath<ServerJsonVersionChangedEvent>("刷新展示的ServerJsonVersion", LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Vm.ServerJsonVersion = NTMinerRoot.Instance.GetServerJsonVersion();
                    });
                });
            });
        }

        private void ScrollViewer_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
