using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Baracuda.Mediator.Events
{
    public class EventChannel : EventChannelBase, IEventChannel
    {
        #region --- Member Variables ---

        /*
         * Constants
         */

        private const int InitialCapacity = 8;

        /*
         * State
         */

        public Action this[int index] => _listener[index];

        [NonSerialized] private int _nextIndex;
        [NonSerialized] private Action[] _listener = new Action[InitialCapacity];

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- Add Listener ---

        public void AddUniqueListener(Action listener)
        {
            if (IsContained(listener))
            {
                return;
            }
            AddListener(listener);
        }

        public void AddListener(Action listener)
        {
            Assert.IsNotNull(listener);

            if (_listener.Length <= _nextIndex)
            {
                IncreaseCapacity();
            }

            _listener[_nextIndex] = listener;

            _nextIndex++;
        }

        private void IncreaseCapacity()
        {
            var increasedArr = new Action[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        private bool IsContained(Action listener)
        {
            for (var i = 0; i < _nextIndex; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region --- Remove Listener ---

        public void RemoveListener(Action listener)
        {
            for (var i = 0; i < _nextIndex; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    RemoveAt(i);
                    return;
                }
            }
        }

        private void RemoveAt(int index)
        {
            --_nextIndex;
            for (var i = index; i < _nextIndex; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[_nextIndex] = null;
        }

        #endregion

        #region --- Raise ---

        public void Raise()
        {
            for (var i = 0; i < _nextIndex; i++)
            {
                _listener[i]();
            }
        }

        #endregion
    }

    public abstract class EventChannel<T> : EventChannelBase, IEventChannel<T>
    {
        #region --- Member Variables ---

        /*
         * Constants
         */

        private const int InitialCapacity = 8;

        /*
         * State
         */

        public Action<T> this[int index] => _listener[index];

        [NonSerialized] private int _nextIndex;
        [NonSerialized] private Action<T>[] _listener = new Action<T>[InitialCapacity];

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- Add Listener ---

        public void AddUniqueListener(Action<T> listener)
        {
            if (IsContained(listener))
            {
                return;
            }
            AddListener(listener);
        }

        public void AddListener(Action<T> listener)
        {
            Assert.IsNotNull(listener);

            if (_listener.Length <= _nextIndex)
            {
                IncreaseCapacity();
            }

            _listener[_nextIndex] = listener;

            _nextIndex++;
        }

        private void IncreaseCapacity()
        {
            var increasedArr = new Action<T>[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        private bool IsContained(Action<T> listener)
        {
            for (var i = 0; i < _nextIndex; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region --- Remove Listener ---

        public void RemoveListener(Action<T> listener)
        {
            for (var i = 0; i < _nextIndex; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    RemoveAt(i);
                    return;
                }
            }
        }

        private void RemoveAt(int index)
        {
            --_nextIndex;
            for (var i = index; i < _nextIndex; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[_nextIndex] = null;
        }

        #endregion

        #region --- Raise ---

        public void Raise(in T value)
        {
            for (var i = 0; i < _nextIndex; i++)
            {
                _listener[i](value);
            }
        }

        #endregion
    }

    public abstract class EventChannel<T1, T2> : EventChannelBase, IEventChannel<T1, T2>
    {
        #region --- Member Variables ---

        /*
         * Constants
         */

        private const int InitialCapacity = 8;

        /*
         * State
         */

        public Action<T1, T2> this[int index] => _listener[index];

        [NonSerialized] private int _nextIndex;
        [NonSerialized] private Action<T1, T2>[] _listener = new Action<T1, T2>[InitialCapacity];

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- Add Listener ---

        public void AddUniqueListener(Action<T1, T2> listener)
        {
            if (IsContained(listener))
            {
                return;
            }
            AddListener(listener);
        }

        public void AddListener(Action<T1, T2> listener)
        {
            Assert.IsNotNull(listener);

            if (_listener.Length <= _nextIndex)
            {
                IncreaseCapacity();
            }

            _listener[_nextIndex] = listener;

            _nextIndex++;
        }

        private void IncreaseCapacity()
        {
            var increasedArr = new Action<T1, T2>[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        private bool IsContained(Action<T1, T2> listener)
        {
            for (var i = 0; i < _nextIndex; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region --- Remove Listener ---

        public void RemoveListener(Action<T1, T2> listener)
        {
            for (var i = 0; i < _nextIndex; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    RemoveAt(i);
                    return;
                }
            }
        }

        private void RemoveAt(int index)
        {
            --_nextIndex;
            for (var i = index; i < _nextIndex; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[_nextIndex] = null;
        }

        #endregion

        #region --- Raise ---

        public void Raise(in T1 first, in T2 second)
        {
            for (var i = 0; i < _nextIndex; i++)
            {
                _listener[i](first, second);
            }
        }

        #endregion
    }

    public abstract class EventChannel<T1, T2, T3> : EventChannelBase, IEventChannel<T1, T2, T3>
    {
        #region --- Member Variables ---

        /*
         * Constants
         */

        private const int InitialCapacity = 8;

        /*
         * State
         */

        public Action<T1, T2, T3> this[int index] => _listener[index];

        [NonSerialized] private int _nextIndex;
        [NonSerialized] private Action<T1, T2, T3>[] _listener = new Action<T1, T2, T3>[InitialCapacity];

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- Add Listener ---

        public void AddUniqueListener(Action<T1, T2, T3> listener)
        {
            if (IsContained(listener))
            {
                return;
            }
            AddListener(listener);
        }

        public void AddListener(Action<T1, T2, T3> listener)
        {
            Assert.IsNotNull(listener);

            if (_listener.Length <= _nextIndex)
            {
                IncreaseCapacity();
            }

            _listener[_nextIndex] = listener;

            _nextIndex++;
        }

        private void IncreaseCapacity()
        {
            var increasedArr = new Action<T1, T2, T3>[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        private bool IsContained(Action<T1, T2, T3> listener)
        {
            for (var i = 0; i < _nextIndex; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region --- Remove Listener ---

        public void RemoveListener(Action<T1, T2, T3> listener)
        {
            for (var i = 0; i < _nextIndex; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    RemoveAt(i);
                    return;
                }
            }
        }

        private void RemoveAt(int index)
        {
            --_nextIndex;
            for (var i = index; i < _nextIndex; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[_nextIndex] = null;
        }

        #endregion

        #region --- Raise ---

        public void Raise(in T1 first, in T2 second, in T3 third)
        {
            for (var i = 0; i < _nextIndex; i++)
            {
                _listener[i](first, second, third);
            }
        }

        #endregion
    }

    public abstract class EventChannel<T1, T2, T3, T4> : EventChannelBase, IEventChannel<T1, T2, T3, T4>
    {
        #region --- Member Variables ---

        /*
         * Constants
         */

        private const int InitialCapacity = 8;

        /*
         * State
         */

        public Action<T1, T2, T3, T4> this[int index] => _listener[index];

        [NonSerialized] private int _nextIndex;
        [NonSerialized] private Action<T1, T2, T3, T4>[] _listener = new Action<T1, T2, T3, T4>[InitialCapacity];

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- Add Listener ---

        public void AddUniqueListener(Action<T1, T2, T3, T4> listener)
        {
            if (IsContained(listener))
            {
                return;
            }
            AddListener(listener);
        }

        public void AddListener(Action<T1, T2, T3, T4> listener)
        {
            Assert.IsNotNull(listener);

            if (_listener.Length <= _nextIndex)
            {
                IncreaseCapacity();
            }

            _listener[_nextIndex] = listener;

            _nextIndex++;
        }

        private void IncreaseCapacity()
        {
            var increasedArr = new Action<T1, T2, T3, T4>[_listener.Length * 2];

            for (var i = 0; i < _listener.Length; i++)
            {
                increasedArr[i] = _listener[i];
            }

            _listener = increasedArr;
        }

        private bool IsContained(Action<T1, T2, T3, T4> listener)
        {
            for (var i = 0; i < _nextIndex; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region --- Remove Listener ---

        public void RemoveListener(Action<T1, T2, T3, T4> listener)
        {
            for (var i = 0; i < _nextIndex; i++)
            {
                if (_listener[i].Equals(listener))
                {
                    RemoveAt(i);
                    return;
                }
            }
        }

        private void RemoveAt(int index)
        {
            --_nextIndex;
            for (var i = index; i < _nextIndex; ++i)
            {
                _listener[i] = _listener[i + 1];
            }

            _listener[_nextIndex] = null;
        }

        #endregion

        #region --- Raise ---

        public void Raise(in T1 first, in T2 second, in T3 third, in T4 fourth)
        {
            for (var i = 0; i < _nextIndex; i++)
            {
                _listener[i](first, second, third, fourth);
            }
        }

        #endregion
    }
}