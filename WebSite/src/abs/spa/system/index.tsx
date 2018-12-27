import dva from 'dva';
import createLoading from 'dva-loading';
// import defaultHistory from './common/http/request/listener';
import createHistory from 'history/createHashHistory';
import registerServiceWorker from '../../../registerServiceWorker'; 
import { RouterConfig } from './Router';  
// import 'amfe-flexible'; 
// import './styles/theme.less';
// import { createLogger } from 'redux-logger';

/**
 *  1. Initialize
 */
const appDva = dva({
  // use default history
  history: createHistory(), 
  // register middleware
  onAction: [
    // createLogger(), // logger publish remove 
  ]
});

/**
 * 2. Register middleware
 */
appDva.use(createLoading());
// appDva.use(createLogger());

/**
 * 3. Model |move to ruoterconfig
 */ 
// appDva.model(countModel);

/**
 * 4. Router Setting
 */
appDva.router(RouterConfig);

/**
 *  5. App Start
 */ 
appDva.start('#root');  

// developer mode | local cache
registerServiceWorker();
