import java.util.concurrent.CountDownLatch;


public class TestRunner implements Runnable {
	private final static int RUN_COMPARE_AND_SET = 1;
	private final static int RUN_INCREMENT = 2;
    private static int loop = 1000000;
    private static Accumulator accumulator;
    private CountDownLatch startSignal;// = new CountDownLatch(1);
    private CountDownLatch doneSignal; // = new CountDownLatch(N);
    private int action;

    private AtomicTest _atomic;

    public static void main(String[] args)
		throws InterruptedException 
	{
        int threadCount = 1;
        if (args.length > 0)
        {
            try
            {
                threadCount = Integer.parseInt(args[0]);
            }
            catch (Throwable e)
            {
                System.out.println(e.getMessage());
            }
        }
        System.out.println("Using " + threadCount + " threads:");

        TestRunner a = new TestRunner(new AtomicIntegerTest());
        TestRunner b = new TestRunner(new MonitorAtomicTest());
        
        a.action = RUN_COMPARE_AND_SET;
        a.runMultiThreadInParallel(threadCount);
        b.action = RUN_COMPARE_AND_SET;
        b.runMultiThreadInParallel(threadCount);
        a.action = RUN_INCREMENT;
        a.runMultiThreadInParallel(threadCount);
        b.action = RUN_INCREMENT;
        b.runMultiThreadInParallel(threadCount);
	}
    
    public TestRunner(AtomicTest atomic)
    {
    	_atomic = atomic;    		
    }
    
    public void run()
    {
        try {
	    	startSignal.await();
	    	runAndRecordElapsed();
        } catch (InterruptedException ex) {} // return;
        doneSignal.countDown();
    }
    
    private void runMultiThreadInParallel(int threadCount)
    	throws InterruptedException 
    {
    	accumulator = new Accumulator();
        startSignal = new CountDownLatch(1);
        doneSignal = new CountDownLatch(threadCount);
        for (int i = 0; i < threadCount; ++i) new Thread(this).start();

        startSignal.countDown();      // let all threads proceed
        doneSignal.await();           // wait for all to finish
        System.out.printf("%25s.%-20s (ns): %6d Average, %6d Minimal, %6d Maxmial\n",
                _atomic.getClass().getName(), getActionDescriptoin(),
                accumulator.getAverage(), accumulator.getMinimal(), accumulator.getMaximal());
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
        accumulator.Accumulate(elapsed);
    }

    private void runCompareAndSet()
    {
        int result1, result2, result3;
        for (int i = loop - 1; i >= 0; i--)
        {
            _atomic.compareAndSet(100, 50);
            result1 = _atomic.get();
            _atomic.compareAndSet(50, 100);
            result2 = _atomic.get();
            _atomic.compareAndSet(100, 50);
            result3 = _atomic.get();
        }
    }

    private void runIncrement()
    {
        int result1, result2, result3;
        for (int i = loop - 1; i >= 0; i--)
        {
            _atomic.incrementAndGet();
            result1 = _atomic.get();
            _atomic.incrementAndGet();
            result2 = _atomic.get();
            _atomic.incrementAndGet();
            result3 = _atomic.get();
        }
    }

}
