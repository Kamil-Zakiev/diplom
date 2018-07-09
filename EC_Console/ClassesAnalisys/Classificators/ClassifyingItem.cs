namespace ClassesAnalisys.Classificators
{
    using System;
    using Analisys.Classificators;
    using ClassesAnalisys;

    public class ClassifyingItem
    {
        private readonly ClassifyingItem _next;
        private readonly CurveClass _curveClass;

        public ClassifyingItem(ClassifyingItem next, Func<CurveBaseInfo, bool> classifyFunc, CurveClass curveClass)
        {
            _next = next;
            _classifyFunc = classifyFunc;
            _curveClass = curveClass;
        }

        private readonly Func<CurveBaseInfo, bool> _classifyFunc;

        public CurveClass Classify(CurveBaseInfo curveBaseInfo)
        {
            if (_classifyFunc(curveBaseInfo))
            {
                return _curveClass;
            }

            return _next.Classify(curveBaseInfo);
        }
    }
}