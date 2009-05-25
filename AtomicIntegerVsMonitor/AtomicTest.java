
public interface AtomicTest {
	public int get();
	public void set(int value);
	public boolean compareAndSet(int expected, int newValue);
	public int incrementAndGet();
	public int decrementAndGet();
}
