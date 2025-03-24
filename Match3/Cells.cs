namespace Match3 {
    public class Cells<T> {
        Board<Cells<T>> grid;
        int x;
        int y;
        T block;

        public Cells(Board<Cells<T>> grid, int x, int y) {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetValue(T block) {
            this.block = block;
        }

        public T GetValue() => block;
    }

}