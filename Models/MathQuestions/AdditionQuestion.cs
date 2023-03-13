using Kotlin.Contracts;

namespace MathGame.Models.MathQuestions
{
    internal class AdditionQuestion : BaseQuestion
    {
        public AdditionQuestion() : base() { }  // explicitly call base constructor

        public override BaseQuestion GenerateQuestion()
        {
            Operand1 = GetRandomNumber();
            Operand2 = GetRandomNumber();

            Answer = Operand1 + Operand2;

            return this;
        }

        protected override char GetOperator()
        {
            return '+';
        }
    }
}