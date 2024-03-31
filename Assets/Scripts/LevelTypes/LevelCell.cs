namespace DefaultNamespace.LevelTypes
{
    public abstract class LevelCell
    {
        public Grid grid;
        
        
        public LevelCell(Grid grid)
        {
            this.grid = grid;
        }
        
        
        public void CreateLevel()
        {
            grid.InstantiateGrid();
            grid.InstantiateAstronaut();
            CreateLevelSpecific();
            grid.CreateHeart();
        }


        public abstract void CreateLevelSpecific();
    }
}