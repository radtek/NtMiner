﻿using System;
using System.Threading;
#if DEBUG
using System.ComponentModel;
#endif

namespace NTMiner.Hub {
#if DEBUG
    public class MessagePath<TMessage> : IMessagePathId, INotifyPropertyChanged {
#else
    public class MessagePath<TMessage> : IMessagePathId {
#endif
        private readonly Action<TMessage> _path;
        private bool _isEnabled;
        private int _viaLimit;

#if DEBUG
        public event PropertyChangedEventHandler PropertyChanged;
#endif

        public static MessagePath<TMessage> AddMessagePath(IMessageHub dispatcher, Type location, string description, LogEnum logType, Action<TMessage> path, Guid pathId, int viaLimit = -1) {
            if (path == null) {
                throw new ArgumentNullException(nameof(path));
            }
            MessagePath<TMessage> handler = new MessagePath<TMessage>(location, description, logType, path, pathId, viaLimit);
            dispatcher.AddMessagePath(handler);
            return handler;
        }

        private MessagePath(Type location, string description, LogEnum logType, Action<TMessage> path, Guid pathId, int viaLimit) {
            if (viaLimit == 0) {
                throw new InvalidProgramException("消息路径的viaLimit不能为0，可以为负数表示不限制通过次数或为正数表示限定通过次数，但不能为0");
            }
            _isEnabled = true;
            MessageType = typeof(TMessage);
            Location = location;
            Path = $"{location.FullName}[{MessageType.FullName}]";
            Description = description;
            LogType = logType;
            _path = path;
            PathId = pathId;
            _viaLimit = viaLimit;
        }

        public int ViaLimit {
            get => _viaLimit;
            private set {
                _viaLimit = value;
#if DEBUG
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViaLimit)));
#endif
            }
        }

        private readonly object _locker = new object();

        internal void DecreaseViaLimit(Action<IMessagePathId> onDownToZero) {
            int newValue = Interlocked.Decrement(ref _viaLimit);
            if (newValue == 0) {
                // ViaLimit递减到0从路径列表中移除该路径
                onDownToZero?.Invoke(this);
            }
        }

        public Guid PathId { get; private set; }
        public Type MessageType { get; private set; }
        public Type Location { get; private set; }
        public string Path { get; private set; }
        public LogEnum LogType { get; private set; }
        public string Description { get; private set; }
        public bool IsEnabled {
            get => _isEnabled;
            set {
                _isEnabled = value;
#if DEBUG
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
#endif
            }
        }

        public void Go(TMessage message) {
            try {
                _path?.Invoke(message);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(Path + ":" + e.Message, e);
                throw;
            }
        }
    }
}