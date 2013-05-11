namespace Iridescent.OrmExpress
{
    public interface IQueryTranslator
    {
        void TranslateCriterias();
        void TranslateOrderClauses();
        void Execute();
    }
}