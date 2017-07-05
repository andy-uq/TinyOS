using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Andy.TinyOS;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Xunit.Sdk;

namespace ClassLibrary1
{
    public static class Assert
    {
        public static void That<T>(T value, IAssertion<T> assertion)
        {
            assertion.Evaluate(value);
        }
        
        public static void That(uint value, IAssertion<int> assertion)
        {
            if (assertion.Evaluate((int )value))
                return;
            
            throw new AssertionException();
        }
        
        public static void That(long value, IAssertion<int> assertion)
        {
            if (assertion.Evaluate((int )value))
                return;
            
            throw new AssertionException();
        }
        
        public static void That(ulong value, IAssertion<long> assertion)
        {
            if (assertion.Evaluate((long )value))
                return;
            
            throw new AssertionException();
        }
    }

    public class AssertionException : Exception
    {
    }

    public interface IAssertion<in T>
    {
        bool Evaluate(T actual);
    }

    public static class Is
    {
        public static CollectionEqualToAssertion<T> EquivalentTo<T>(IEnumerable<T> value) => new CollectionEqualToAssertion<T>(value, EqualityComparer<T>.Default);
        public static EqualToAssertion<T> EqualTo<T>(T value) => new EqualToAssertion<T>(value, EqualityComparer<T>.Default);
        public static SameAsAssertion<T> SameAs<T>(T value) => new SameAsAssertion<T>(value);
        public static IsInstanceOf<T> InstanceOf<T>() => new IsInstanceOf<T>();
        public static EmptyAssertion Empty => new EmptyAssertion();
        
        public static SameAsAssertion<object> Null => new SameAsAssertion<object>(null);

        public static NotAssertions Not { get; } = new NotAssertions();    
    }

    public class CollectionEqualToAssertion<T> : IAssertion<IEnumerable<T>>
    {
        private readonly IEnumerable<T> _expected;
        private readonly EqualityComparer<T> _comparer;

        public CollectionEqualToAssertion(IEnumerable<T> expected, EqualityComparer<T> comparer)
        {
            _expected = expected;
            _comparer = comparer;
        }

        public bool Evaluate(IEnumerable<T> actual)
        {
            var hashSet = new HashSet<T>(_expected, _comparer);
            return hashSet.SetEquals(actual);
        }
    }

    public class EmptyAssertion : IAssertion<IEnumerable>
    {
        public bool Evaluate(IEnumerable actual)
        {
            var e = actual.GetEnumerator();
            try
            {
                return e.MoveNext() == false;
            }
            finally
            {
                if (e is IDisposable disposable)
                    disposable.Dispose();
            }
        }
    }

    public static class Contains
    {
        public static ContainsAssertion<T> Item<T>(T value) => new ContainsAssertion<T>(value, EqualityComparer<T>.Default);
    }

    public class ContainsAssertion<T> : IAssertion<IEnumerable<T>>
    {
        private readonly T _expected;
        private readonly EqualityComparer<T> _comparer;

        public ContainsAssertion(T expected, EqualityComparer<T> comparer)
        {
            _expected = expected;
            _comparer = comparer;
        }

        public bool Evaluate(IEnumerable<T> actual)
        {
            return actual.Contains(_expected, _comparer);
        }
    }

    public class NotAssertions
    {
        public IAssertion<T> EqualTo<T>(T value) => Not(Is.EqualTo(value));
        public IAssertion<T> InstanceOf<T>() => Not(Is.InstanceOf<T>());
        public IAssertion<IEnumerable> Empty => Not(Is.Empty);
        public IAssertion<object> Null => Not(Is.Null);

        private static IAssertion<T> Not<T>(IAssertion<T> assertion) => new NotAssertion<T>(assertion);

        private class NotAssertion<T> : IAssertion<T>
        {
            private readonly IAssertion<T> _assertion;

            public NotAssertion(IAssertion<T> assertion)
            {
                _assertion = assertion;
            }

            public bool Evaluate(T actual)
            {
                return !_assertion.Evaluate(actual);
            }
        }
    }


    public class IsInstanceOf<T> : IAssertion<T>
    {
        public bool Evaluate(T actual)
        {
            return true;
        }
    }

    public class EqualToAssertion<T> : IAssertion<T>
    {
        private readonly T _expected;
        private readonly IEqualityComparer<T> _comparer;

        public EqualToAssertion(T expected, IEqualityComparer<T> comparer)
        {
            _expected = expected;
            _comparer = comparer;
        }

        public EqualToAssertion<T> Using(IEqualityComparer<T> comparer)
        {
            return new EqualToAssertion<T>(_expected, comparer);
        }

        public bool Evaluate(T actual)
        {
            return _comparer.Equals(actual, _expected);
        }
    }

    public class SameAsAssertion<T> : IAssertion<T>
    {
        private readonly T _expected;

        public SameAsAssertion(T expected)
        {
            _expected = expected;
        }

        public EqualToAssertion<T> Using(IEqualityComparer<T> comparer)
        {
            return new EqualToAssertion<T>(_expected, comparer);
        }

        public bool Evaluate(T actual)
        {
            return ReferenceEquals(actual, _expected);
        }
    }
    
}