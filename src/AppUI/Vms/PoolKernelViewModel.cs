﻿using NTMiner.Core;
using NTMiner.Views.Ucs;
using System;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class PoolKernelViewModel : EntityViewModelBase<IPoolKernel, Guid>, IPoolKernel, IEditableViewModel {
        private Guid _poolId;
        private Guid _kernelId;
        private string _args;
        private string _description;
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public PoolKernelViewModel(IPoolKernel data) : this(data.GetId()) {
            _poolId = data.PoolId;
            _kernelId = data.KernelId;
            _args = data.Args;
            _description = data.Description;
        }

        public PoolKernelViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (NTMinerRoot.Current.PoolKernelSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdatePoolKernelCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                PoolKernelEdit.ShowWindow(formType ?? FormType.Edit, this);
            });
        }

        public Guid PoolId {
            get {
                return _poolId;
            }
            set {
                if (_poolId != value) {
                    _poolId = value;
                    OnPropertyChanged(nameof(PoolId));
                }
            }
        }

        public string PoolName {
            get {
                return PoolVm.Name;
            }
        }

        private PoolViewModel _poolVm;
        public PoolViewModel PoolVm {
            get {
                if (_poolVm == null || this.PoolId != _poolVm.Id) {
                    PoolViewModels.Current.TryGetPoolVm(this.PoolId, out _poolVm);
                    if (_poolVm == null) {
                        _poolVm = PoolViewModel.Empty;
                    }
                }
                return _poolVm;
            }
        }

        public CoinKernelViewModel CoinKernelVm {
            get {
                var item = NTMinerRoot.Current.CoinKernelSet.FirstOrDefault(a => a.KernelId == this.KernelId && a.CoinId == this.PoolVm.CoinId);
                if (item != null) {
                    return new CoinKernelViewModel(item);
                }
                return null;
            }
        }

        public Guid KernelId {
            get => _kernelId;
            set {
                if (_kernelId != value) {
                    _kernelId = value;
                    OnPropertyChanged(nameof(KernelId));
                }
            }
        }

        public string DisplayName {
            get {
                return $"{Kernel.Code}{Kernel.Version}";
            }
        }

        public KernelViewModel Kernel {
            get {
                KernelViewModel kernel;
                if (KernelViewModels.Current.TryGetKernelVm(this.KernelId, out kernel)) {
                    return kernel;
                }
                return KernelViewModel.Empty;
            }
        }

        public string Args {
            get { return _args; }
            set {
                if (_args != value) {
                    _args = value;
                    OnPropertyChanged(nameof(Args));
                    if (MinerProfileViewModel.Current.CoinId == this.PoolVm.CoinId) {
                        NTMinerRoot.RefreshArgsAssembly.Invoke();
                    }
                }
            }
        }

        public string Description {
            get => _description;
            set {
                if (_description != value) {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
    }
}
