namespace MathGame.Models.MathQuestions
{
    internal class SubstructionQuestion : BaseQuestion
    {
        public SubstructionQuestion() : base() { }  // explicitly call base constructor

        public override void GenerateQuestion()
        {
            Operand1 = GetRandomNumber();
            Operand2 = GetRandomNumber();

            Answer = Operand1 - Operand2;
        }

        protected override char GetOperator()
        {
            return '-';
        }
    }
}