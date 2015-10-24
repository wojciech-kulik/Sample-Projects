#pragma once

template <class T, int PoolSize>
class ObjectPool
{
	typedef bool (*conditionFoo)(T*);

private:
	void SetObject(T * object);
protected:
	int _poolIndex;
	T* * _pool;
	conditionFoo _overrideConditon;
public:
	ObjectPool(conditionFoo overrideConditon = NULL);
	~ObjectPool();
	void Add(T * object);
	void Clear();
	int Size();

	T * operator[](int index);
	const T * operator[](int index) const;
};



//IMPLEMENTATION=================================================================
//===============================================================================
template <class T, int PoolSize>
ObjectPool<T, PoolSize>::ObjectPool(conditionFoo overrideConditon = NULL) : 
	_poolIndex(-1), _overrideConditon(overrideConditon)
{
	_pool = new T*[PoolSize];
	for (int i = 0; i < PoolSize; i++)
		_pool[i] = NULL;
}
	
template <class T, int PoolSize>
ObjectPool<T, PoolSize>::~ObjectPool()
{
	for (int i = 0; i < PoolSize; i++)
		delete _pool[i];
	delete[] _pool;
}

template <class T, int PoolSize>
void ObjectPool<T, PoolSize>::SetObject(T * object)
{
	if (_pool[_poolIndex] != NULL)
		delete _pool[_poolIndex];
	_pool[_poolIndex] = object;
}
	
template <class T, int PoolSize>
void ObjectPool<T, PoolSize>::Add(T * object)
{
	_poolIndex = (_poolIndex + 1) % PoolSize;

	if (_overrideConditon != NULL && _pool[_poolIndex] != NULL)
	{
		int oldIndex = _poolIndex;

		do
		{
			if (_pool[_poolIndex] == NULL || (*_overrideConditon)(_pool[_poolIndex]))
			{
				SetObject(object);
				return;
			}	
			else
			{
				_poolIndex = (_poolIndex + 1) % PoolSize;
			}
		} while (oldIndex != _poolIndex);
	}
	else
	{
		SetObject(object);
	}
}
	
template <class T, int PoolSize>
void ObjectPool<T, PoolSize>::Clear()
{
	_poolIndex = 0;
	for (int i = 0; i < PoolSize; i++)
	{
		delete _pool[i];
		_pool[i] = NULL;
	}
}

template <class T, int PoolSize>
int ObjectPool<T, PoolSize>::Size()
{
	return PoolSize;
}

template <class T, int PoolSize>
T * ObjectPool<T, PoolSize>::operator[](int index) 
{
	return _pool[index];
}
 
template <class T, int PoolSize>
const T * ObjectPool<T, PoolSize>::operator[](int index) const 
{
	return _pool[index];
}