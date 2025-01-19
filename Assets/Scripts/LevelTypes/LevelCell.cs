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
            GridSpecificLevel();
            grid.InstantiateGrid();
            grid.InstantiateAstronaut();
            CreateLevelSpecific();
            grid.CreateHeart();
        }


        public abstract void CreateLevelSpecific();
        public abstract void GridSpecificLevel();

        public abstract bool EndCondition();
    }
}