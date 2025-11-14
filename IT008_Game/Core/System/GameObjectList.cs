using IT008_Game.Core.Components;
using System.Collections;

namespace IT008_Game.Core.System
{
    /// <summary>
    /// A list of gameobjects, with added custom behavior for destroying/adding gameobjects
    /// </summary>
    internal class GameObjectList : IList<GameObject>
    {
        readonly List<GameObject> _internal;

        readonly List<GameObject> _addList;

        private bool haveDestroy = false;

        public GameObjectList()
        {
            _internal = [];
            _addList = [];
            haveDestroy = false;
        }


        /// <summary>
        /// Recursively destroy all marked children of a child.
        /// </summary>
        void DoDestroy()
        {
            if (_internal.Count == 0)
                return;

            foreach (var item in _internal)
            {
                if (item.WillDestroyNextFrame)
                {
                    item.Children.DoDestroy();
                    item.Children.RemoveAll(x =>
                    {
                        x.OnDestroy();
                        return x.WillDestroyNextFrame;
                    });
                }
            }

        }
        public void Update()
        {
            if (_addList.Count > 0)
            {
                _internal.AddRange(_addList);
                _addList.Clear();
            }

            // Try to destroy gameobject
            if (haveDestroy)
            {
                _internal.RemoveAll(x =>
                {
                    if (x.WillDestroyNextFrame)
                    {
                        x.Children.DoDestroy();
                        x.OnDestroy();
                        return true;
                    }

                    return false;
                });
                haveDestroy = false;
            }

            foreach (var item in _internal)
            {
                if (item.WillDestroyNextFrame)
                {
                    haveDestroy = true;
                }
                item.Update();
            }
        }
        public void Draw(Graphics g)
        {
            foreach (var item in _internal)
            {
                item.Draw(g);
            }
        }
        public void Add(GameObject item)
        {
            item.Parent = this;
            _addList.Add(item);
        }

        public void AddRange(IEnumerable<GameObject> collection)
        {
            foreach (var item in collection)
            {
                item.Parent = this;
                _addList.Add(item);
            }
        }

        #region List Default
        public GameObject this[int index]
        {
            get => _internal[index];
            set => _internal[index] = value;
        }

        public int Count => _internal.Count;
        public bool IsReadOnly => false;


        public void Clear() => _internal.Clear();
        public bool Contains(GameObject item) => _internal.Contains(item);
        public void CopyTo(GameObject[] array, int arrayIndex) => _internal.CopyTo(array, arrayIndex);
        public IEnumerator<GameObject> GetEnumerator() => _internal.GetEnumerator();
        public int IndexOf(GameObject item) => _internal.IndexOf(item);
        public void Insert(int index, GameObject item) => _internal.Insert(index, item);
        public bool Remove(GameObject item) => _internal.Remove(item);
        public void RemoveAll(Predicate<GameObject> predicate)
        {
            _internal.RemoveAll(predicate);
        }
        public void RemoveAt(int index) => _internal.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
