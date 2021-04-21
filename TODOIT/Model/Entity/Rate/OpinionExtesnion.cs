using TODOIT.ViewModel.Opinion;

namespace TODOIT.Model.Entity.Rate
{
    public static class OpinionExtesnion
    {
        public static void Assign(this Opinion opinion, OpinionViewModel model)
        {
            opinion.Comment = model.Comment;
            opinion.Quality = model.Quality;
            opinion.Salary = model.Salary;
        }
    }
}