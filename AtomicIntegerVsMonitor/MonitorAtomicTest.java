
public class MonitorAtomicTest implements AtomicTest {
	private volatile int _value;
	public synchronized boolean compareAndSet(int expected, int newValue) {
		if (expected==_value) {
			_value = newValue;
			return true;
		}
		return false;
	}

	public synchronized int decrementAndGet() {
		return --_value;
	}

	public int get() {
		return _value;
	}

	public synchronized int incrementAndGet() {
		return ++_value;
	}

	public synchronized void set(int value) {
		_value = value;
	}

}
