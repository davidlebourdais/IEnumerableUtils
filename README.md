# IEnumerable Utils
[![Build Status](https://dev.azure.com/davidlebourdais/IEnumerableUtils/_apis/build/status/davidlebourdais.IEnumerableUtils?branchName=master)](https://dev.azure.com/davidlebourdais/IEnumerableUtils/_build/latest?definitionId=10&branchName=master)

Contains a set of extension methods to manipulate IEnumerable<T> or IList<T> objects easier.

> For now there is only one helper: SyncWith. However, the unit test structure might be reused to develop and test new helpers.

## SyncWith
Synchronizes every items of the collection with another one used as a reference. If collection types are not interchangeable, you can provide a converter using the 'converter' parameter.
 
## License
This work is licensed under the [MIT License](LICENSE.md).
