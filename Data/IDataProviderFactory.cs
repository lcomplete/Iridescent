namespace Iridescent.Data
{
	/// <summary>
	/// Summary description for IDataAccessProviderFactory.
	/// </summary>
	public interface IDataProviderFactory
	{
        IDataContext GetDataContext();
	}
}
