namespace MathGame.Models.MathQuestions
{
    internal class DivisionQuestion : BaseQuestion
    {
        public DivisionQuestion() : base() { }  // explicitly call base constructor

        public override void GenerateQuestion()
        {
            Operand1 = GetRandomNumber();

            do
            {
                Operand2 = GetRandomNumber();
            } while (Operand2 == 0);  // second operand must be not 0

            Answer = System.Math.Round((double)Operand1 / Operand2, 2);
        }

        protected override char GetOperator()
        {
            return '/';
        }
    }
}