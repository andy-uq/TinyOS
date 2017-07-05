using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Common;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace ClassLibrary1
{
    public static class MoreFluentAssertions
    {
        public static AndConstraint<ObjectAssertions> Be<T>(this ObjectAssertions subject, T expected, IEqualityComparer<T> comparer, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion.BecauseOf(because, becauseArgs).ForCondition(subject.Subject.IsSameOrEqualTo(expected, comparer)).FailWith("Expected {context:object} to be {0}{reason}, but found {1}.", expected, subject);
            return new AndConstraint<ObjectAssertions>(subject);
        }

        public static AndConstraint<GenericCollectionAssertions<T>> Be<T>(this GenericCollectionAssertions<T> subject, IEnumerable<T> expected, IEqualityComparer<T> comparer = null, string because = "", params object[] becauseArgs)
        {
            if (expected == null)
                throw new NullReferenceException("Cannot verify collection equality against a <null> collection.");

            if (subject.Subject == null)
            {
                Execute.Assertion.BecauseOf(because, becauseArgs)
                    .FailWith("Expected {context:collection} to contain {0} in order{reason}, but found <null>.", (object) expected);

                return new AndConstraint<GenericCollectionAssertions<T>>(subject);
            }
            
            comparer = comparer ?? EqualityComparer<T>.Default;

            using (var a = subject.Subject.GetEnumerator())
            using (var b = expected.GetEnumerator())
            {
                int index = 0;
                while (a.MoveNext())
                {
                    var expectedItem = a.Current;

                    if (b.MoveNext() && comparer.Equals(b.Current, expectedItem))
                    {
                        index++;
                        continue;
                    }

                    Execute.Assertion.BecauseOf(because, becauseArgs)
                        .FailWith("Expected {context:collection} {0} to be {1} in order{reason}, but {2} (index {3}) did not appear (in the right order).", (object) subject.Subject, (object) expected, expectedItem, (object) index);
                }

                if (b.MoveNext())
                    Execute.Assertion.BecauseOf(because, becauseArgs)
                        .FailWith("Expected {context:collection} {0} to be {1} in order{reason}, but there were more items than expected.", (object) subject.Subject, (object) expected);
            }

            return new AndConstraint<GenericCollectionAssertions<T>>(subject);
        }

        public static bool IsSameOrEqualTo<T>(this object actual, T expected, IEqualityComparer<T> comparer)
        {
            if (actual == null && expected == null)
                return true;
            
            if (actual is T typed)
                return comparer.Equals(typed, expected);
            
            return false;
        }
    }
}