namespace FinderOfStandarts.Models
{
    class Feature
    {
        public bool IsContinuous { get; set; }
        public bool IsActive { get; set; }
        public bool IsClass { get; set; }
        public string Name { get; set; }

        public override string ToString(){
            return $"Feature{{\"{Name}\" is_active={IsActive}, is_continuous={IsContinuous}, is_class={IsClass}}}";
        }
    }
}