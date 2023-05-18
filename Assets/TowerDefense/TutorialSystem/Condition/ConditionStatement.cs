using System;

namespace TowerDefense.TutorialSystem {
    enum ConditionStatement {
        EqualTo, GreaterThan, LessThan, GreaterThanOrEqualTo, LessThanOrEqualTo
    }
    internal static class ConditionStatementExtensions {
        public static bool Matches(this ConditionStatement condition, Single valueLeft, Single valueRight) {
            return condition switch {
                ConditionStatement.EqualTo => Math.Abs( valueLeft - valueRight ) < 0.1f,
                ConditionStatement.GreaterThan => valueLeft > valueRight,
                ConditionStatement.LessThan => valueLeft < valueRight,
                ConditionStatement.GreaterThanOrEqualTo => valueLeft >= valueRight,
                ConditionStatement.LessThanOrEqualTo => valueLeft <= valueRight,
                _ => false
            };
            
        }
    }
}