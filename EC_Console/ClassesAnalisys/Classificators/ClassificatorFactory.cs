namespace ClassesAnalisys.Classificators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Analisys.Classificators;
    using ClassesAnalisys;

    public static class ClassificatorFactory
    {
        public static Classificator Create(IReadOnlyList<Func<CurveBaseInfo, bool>> conditions)
        {
            conditions = conditions.Reverse().ToArray();
            ClassifyingItem chain = null;
            for (var i = 0; i < conditions.Count; i++)
            {
                var curveClass = new CurveClass() {ClassNumber = conditions.Count - i};
                chain = new ClassifyingItem(chain, conditions[i], curveClass);
            }

            return new Classificator(chain);
        }
    }
}