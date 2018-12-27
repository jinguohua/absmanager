import dva from 'dva';
import createLoading from 'dva-loading';
import createHistory from 'history/createHashHistory';
import registerServiceWorker from '../../../registerServiceWorker'; 
import { RouterConfig } from './Router';  
// import 'amfe-flexible'; 
// import './styles/theme.less';
// import { createLogger } from 'redux-logger';
 
/**
 *  1. Initialize 
 */
const app = dva({
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
app.use(createLoading());
// appDva.use(createLogger());

/**
 * 3. Model |move to ruoterconfig
 */ 
// appDva.model(countModel);

/**
 * 4. Router Setting
 */
app.router(RouterConfig);

/**
 *  5. App Start
 */ 
app.start('#root');  

// developer mode | local cache
registerServiceWorker();
