namespace gMKVToolNix
{
    public class OptionValue<T>
    {
        public T Option { get; set; }

        public string Parameter { get; set; }

        public OptionValue(T opt, string par)
        {
            Option = opt;
            Parameter = par;
        }
    }
}
