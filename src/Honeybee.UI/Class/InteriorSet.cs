namespace Honeybee.UI
{
    public class InteriorSet
    {
        public string Wall { get; set; }
        public string Ceiling { get; set; }
        public string Floor { get; set; }
        public string Window { get; set; }
        public string Door { get; set; }
        public string GlassDoor { get; set; }

        public InteriorSet()
        {
        }

        public InteriorSet Duplicate()
        {
            var obj = new InteriorSet();   
            obj.Wall = Wall;
            obj.Floor = Floor;
            obj.Door = Door;
            obj.GlassDoor = GlassDoor;
            obj.Ceiling = Ceiling;
            obj.Window = Window;
            return obj;
        }
    }



}
