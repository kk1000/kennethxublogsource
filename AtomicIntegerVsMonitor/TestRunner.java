import java.util.concurrent.BrokenBarrierException;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.CyclicBarrier;

public class TestRunner {
	private final static int RUN_COMPARE_AND_SET = 1;
	private final static int RUN_INCREMENT = 2;
    private static int loop = 1000000;
    private static int threadCount = 1;
    private static boolean verbose;
    
    private CyclicBarrier  barrier;
    private CountDownLatch doneSignal;
    private Accumulator accumulator;
    private int action;
    private AtomicTest atomic;

    public static void main(String[] args)
		throws InterruptedException 
	{
        if (args.length > 0) threadCount = Integer.parseInt(args[0]);
        verbose = "true".equalsIgnoreCase(System.getProperty("verbose"));
        TestRunner a = new TestRunner(new AtomicIntegerTest());
        TestRunner b = new TestRunner(new MonitorAtomicTest());

        a.runCompareAndSetInParallel();
        b.runCompareAndSetInParallel();
        a.runIncrementInParallel();
        b.runIncrementInParallel();
	}
    
    public TestRunner(AtomicTest atomic)
    {
    	this.atomic = atomic;    		
    }

    private void runCompareAndSetInParallel()
    	throws InterruptedException 
    {
    	atomic.set(50);
        action = RUN_COMPARE_AND_SET;
        runMultiThreadInParallel();
    }
    
    private void runIncrementInParallel()
		throws InterruptedException 
	{
    	atomic.set(0);
        action = RUN_INCREMENT;
        runMultiThreadInParallel();
        if (atomic.get() != loop * 3 * threadCount)
        {
        	throw new RuntimeException("Increment count error.");
        }
	}
    
    private void runMultiThreadInParallel()
		throws InterruptedException 
	{
		accumulator = new Accumulator();
	    barrier = new CyclicBarrier(threadCount);
	    doneSignal = new CountDownLatch(threadCount);
	    Runnable work = new Runnable() {
			public void run() {
		        try {
		        	barrier.await();
			    	runAndRecordElapsed();
		        } 
		        catch (InterruptedException ex) {} // return;
		    	catch (BrokenBarrierException ex) {}
		        doneSignal.countDown();
			}
	    };
	    for (int i = 0; i < threadCount; ++i) {
	    	new Thread(work, "T" + i).start();
	    }
	
	    doneSignal.await();           // wait for all to finish
		System.out.printf(
			"%19s.%-16s (ns):%6d Average,%6d Minimal,%6d Maxmial,%3d Threads\n",
	        atomic.getClass().getName(), getActionDescriptoin(),
	        accumulator.getAverage(), accumulator.getMinimal(), 
	        accumulator.getMaximal(), accumulator.getCount());
	}

    private String getActionDescriptoin()
    {
        switch(action)
        {
        case RUN_COMPARE_AND_SET:
        	return "runCompareAndSet";
        case RUN_INCREMENT:
        	return "runIncrement";
        }
        return null;
    }
	
    private void runAndRecordElapsed()
    {
        long start = System.currentTimeMillis();
        switch(action)
        {
        case RUN_COMPARE_AND_SET:
        	runCompareAndSet();
        	break;
        case RUN_INCREMENT:
        	runIncrement();
        	break;
        }
        long elapsed = System.currentTimeMillis() - start;
        accumulator.accumulate(elapsed);
	    if (verbose) {
	    	System.err.println(Thread.currentThread().getName() + 
	    			" start:" + start + " elapsed:" + elapsed);
	    }
    }

    private void runCompareAndSet()
    {
        int result1, result2, result3;
        for (int i = loop - 1; i >= 0; i--)
        {
            atomic.compareAndSet(100, 50);
            result1 = atomic.get();
            atomic.compareAndSet(50, 100);
            result2 = atomic.get();
            atomic.compareAndSet(100, 50);
            result3 = atomic.get();
        }
    }

    private void runIncrement()
    {
        int result1, result2, result3;
        for (int i = loop - 1; i >= 0; i--)
        {
            atomic.incrementAndGet();
            result1 = atomic.get();
            atomic.incrementAndGet();
            result2 = atomic.get();
            atomic.incrementAndGet();
            result3 = atomic.get();
        }
    }

}
