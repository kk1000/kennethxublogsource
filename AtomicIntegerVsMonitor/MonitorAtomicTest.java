
public class MonitorAtomicTest implements AtomicTest {
	private volatile int _value;
	public synchronized boolean compareAndSet(int expected, int newValue) {
		int value = _value;
		boolean same = (expected==value);
		if (same) _value = newValue;
		return same;
	}

	public synchronized int decrementAndGet() {
		int value = _value;
		_value = ++value;
		return value;
	}

	public int get() {
		return _value;
	}

	public synchronized int incrementAndGet() {
		int value = _value;
		_value = ++value;
		return value;
	}

	public synchronized void set(int value) {
		_value = value;
	}

}
