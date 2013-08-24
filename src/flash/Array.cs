using System;
using System.Collections.Generic;
using System.Text;

namespace flash
{
	// VectorとArrayほぼ共通だが、aliasなど使用できないので、別々に定義
    public class Array<T> : List<T> {
        public Array() { }
//        public Array(IEnumerable<T> collection) : base(collection) { }
        public Array(int capacity) : base(capacity) { }
#if false
        public override string ToString() {
            _sb.Clear();
            bool isFirst = true;
            foreach(var item in this) {
                if (!isFirst)
                    _sb.Append(',');
                _sb.Append(item.ToString());

                isFirst = false;
            }

            return _sb.ToString();
        }
#endif

        public int length {
            get { return Count; }
            set {
                if( value == 0 ){
					Clear();
				} else {
					App.Util.Assert(false);		//unsupported
				}
            }
        }

        public T pop() {
			if( Count < 1 ){
				return default(T);
			}
            var last = this[Count - 1];
            RemoveAt(Count - 1);

            return last;
        }

        public int push(T item) {
            Add(item);

            return Count;
        }

        public T shift() {
			if( Count < 1 ){
				return default(T);
			}
            var first = this[0];
            RemoveAt(0);

            return first;
        }

        public Array<T> slice(int startIndex = 0, int endIndex = 16777215) {
            int start = startIndex >= 0 ? startIndex : Count + startIndex;
            int end = endIndex == 16777215 ? Count : endIndex >= 0 ? endIndex : Count + endIndex;

            var array = new Array<T>(end - start);

            for(int i = start; i < end; i++) {
                array.Add(this[i]);
            }

            return array;
        }

        public virtual Array<T> splice(int startIndex, uint deleteCount, params T[] p) {
            var array = new Array<T>();
            for(int i = 0; i < deleteCount; i++) {
                array.push(this[startIndex + i]);
            }

            this.RemoveRange(startIndex, (int)deleteCount);

            if (p != null) {
                InsertRange(startIndex, p);
            }

            return array;
        }
    }
}

// Local Variables:
// coding: utf-8
// End:
