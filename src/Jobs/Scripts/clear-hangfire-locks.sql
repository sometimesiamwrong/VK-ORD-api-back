-- Script to clear stale Hangfire distributed locks in PostgreSQL
-- Use this if you encounter PostgreSqlDistributedLockException with "Lock timeout" error
-- This can happen if a previous instance of the application crashed without releasing locks

-- 1. Check current locks
SELECT * FROM hangfire.lock ORDER BY acquired DESC;

-- 2. Clear all locks (use with caution - only when no Hangfire instances are running)
-- DELETE FROM hangfire.lock;

-- 3. Clear locks older than 5 minutes (safer option)
DELETE FROM hangfire.lock 
WHERE acquired < NOW() - INTERVAL '5 minutes';

-- 4. Clear specific lock for recurring job
-- DELETE FROM hangfire.lock 
-- WHERE resource = 'hangfire:lock:recurring-job:sync-erir-statuses';

-- 5. Verify locks are cleared
SELECT * FROM hangfire.lock ORDER BY acquired DESC;

