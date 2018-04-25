namespace Analisys.Classificators
{
    using System;

    public class FieldOrderBasedClassificator//: IClassificator
    {
        public CurveClass Classify(CurveBaseInfo curveInfo)
        {
            var result = new CurveClass();
            
            if (curveInfo.EdgeB <= Math.Pow(curveInfo.FieldOrder, 0.25))
            {
                result.ClassNumber = 1;
            }
            else if (curveInfo.EdgeB <= Math.Pow(curveInfo.FieldOrder, 0.33))
            {
                result.ClassNumber = 2;
            }
            else if (curveInfo.EdgeB <= Math.Pow(curveInfo.FieldOrder, 0.5))
            {
                result.ClassNumber = 3;
            }
            else
            {
                result.ClassNumber = 4;
            }
            
            return result;
        }
    }
}