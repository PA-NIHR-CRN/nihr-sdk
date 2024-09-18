namespace BPOR.Rms.Models.Researcher
{
    public abstract class FormWithSteps
    {
        public int Step { get; set; } = 1;
        public abstract int TotalSteps { get; }
        public bool Completed { get; set; }
        public abstract string StepName { get; }

        public void GotoNextStep()
        {
            GotoNextStep(Step + 1);
        }

        public void GotoNextStep(int step)
        {
            if (Completed)
            {
                Step = TotalSteps;
            }
            else
            {
                Step = step;
            }


            if (Step >= TotalSteps)
            {
                Completed = true;
            }
        }
    }
}