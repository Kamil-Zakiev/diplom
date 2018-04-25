namespace Analisys.Classificators
{
    public class Classificator
    {
        public Classificator(ClassifyingItem classifyingItem)
        {
            ClassifyingItem = classifyingItem;
        }

        private ClassifyingItem ClassifyingItem { get; }

        public CurveClass Classify(CurveBaseInfo curveInfo)
        {
            var result = new CurveClass();
            if (curveInfo.EdgeB <= 100)
            {
                result.ClassNumber = 1;
            }
            else if (curveInfo.EdgeB <= 1000)
            {
                result.ClassNumber = 2;
            }
            else if (curveInfo.EdgeB <= 10000)
            {
                result.ClassNumber = 3;
            }
            else
            {
                result.ClassNumber = 4;
            }

            return ClassifyingItem.Classify(curveInfo);
        }
    }
}