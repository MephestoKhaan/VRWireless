package com.MephestoKhaan.Movemment;

import java.util.LinkedList;
import java.util.Queue;

public class MeanQueue
{
	private Queue<Long> queue_ = new LinkedList<Long>();
	
	int size_ = 10;
	float sum_ =0;
	float error_ = 0.6f;

	
	void Set(MeanQueue q)
	{
		Clear();
		long a;
		int size = q.GetSize();
		for(int i = 0; i < size; i++)
		{
			a = q.Take();
			if(a!=-1)
			{
				Add(a);
			}
		}
		error_ = q.error_;
		size_ = q.size_;
	}
	
	long[] GetMargin()
	{
		float mean = GetMean();
		long a = (long) (mean-mean*error_);
		long b = (long) (mean+mean*error_);
		if(b<a)
		{
			long aux = a;
			a= b;
			b=aux;
		}
		long[] ret = new long[2];
		ret[0] = a;
		ret[1] = b;
		return ret;
	}
	
	long Take()
	{
		long p = -1;
		if(queue_.size()>0)
		{
			p =queue_.remove();
			sum_-= p;
		}
		return p;
	}
	
	void Add(long p)
	{
		sum_ += p;
		queue_.add(p);
		
		if(queue_.size()==size_)
		{
			long q;
			q = queue_.remove();
			sum_-= q;
		}
	}
	
	float GetMean()
	{
		return sum_/(float)queue_.size();
	}
	
	boolean Similar(long  p)
	{
		long a, b;
		long[] size = GetMargin();
		a = size[0];
		b = size[1];
		if(queue_.size() == 0) //Si el buffer está vacío
		{
			return true;
		}
		if(p>=a && p<=b)
		{
			return true;
		}
		return false;
	}
	
	float AccuracyOf(long p)
	{
		float mean = GetMean();
		if(mean != 0 && mean == mean && p != 0)
		{
			return Math.abs(p/mean -1) ;
		}
		return 1.0f;
	}
	int GetSize()
	{
		return queue_.size();
	}
	void Clear()
	{
		queue_.clear();
		sum_ = 0;
	}
}