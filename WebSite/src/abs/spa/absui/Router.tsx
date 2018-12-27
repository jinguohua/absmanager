import * as React from 'react';
import { Router, Route, Switch, Redirect } from 'dva/router';
import routerUtil from '../../../utils/routerUtil';
import identity from '../../config/routerIdentityConfig';
import { menuKey } from '../../config/navigationMenuKeyConfig';

export class App extends React.Component<any, any> {
  render() {
    const { app }: any = this.props;
    const routerConfig = {
      '/': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui', 'global'], () =>
          import('../../../layouts/SideBarLayout')
        )
      },
      '/errorpage': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSErrorPage')
        )
      },
      '/switch': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSSwitch')
        )
      }, 
      '/button': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSButton')
        )
      },
      '/paragraph': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSParagraph')
        )
      },
      '/emphasize-text': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSEmphasizeText')
        )
      },
      '/label-value-list': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSLabelValueList')
        )
      },
      '/icon': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSIcon')
        )
      },
      '/icon-text': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSIconText')
        )
      },
      '/progress': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSProgress')
        )
      },
      '/registered-process': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSRegisteredProcess')
        )
      },
      '/panel': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSPanel')
        )
      },
      '/simple-image': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSImage')
        )
      },
      '/anchor-point': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSAnchorPoint')
        )
      },
      '/bubble': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSBubble')
        )
      },
      '/alert': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSAlert')
        )
      },
      '/modal': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSModal')
        )
      },
      '/avatar': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSAvatar')
        )
      },
      '/message': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSMessage')
        )
      },
      '/input': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSInput')
        )
      },
      '/select': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSSelect')
        )
      },
      '/calendar': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSDatePicker')
        )
      },
      '/tag': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSTag')
        )
      },
      '/list': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui', 'product'], () =>
          import('../../views/absui/ABSList')
        )
      },
      '/product-page-title': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSProductPageTitle')
        )
      },
      '/url-list': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSLinkCardList')
        )
      },
      '/search-bar': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui', 'global'], () =>
          import('../../views/absui/ABSSearchBar')
        )
      },
      '/structure': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSStructure')
        )
      },
      '/upload': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSUploadCard')
        )
      },
      '/radio': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSRadio')
        )
      },
      '/checkbox': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSCheckbox')
        )
      },
      '/tab': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSTab')
        )
      },
      '/comment': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSComment')
        )
      },
      '/no-content': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSNoContent')
        )
      },
      '/filtered-list': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSFilterPanel')
        )
      },
      '/filtered': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSFilter')
        )
      },
      '/pagination': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSPagination')
        )
      },
      '/loading': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSLoading')
        )
      },
      '/steps': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSSteps')
        )
      },
      '/security-list': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui', 'product'], () =>
          import('../../views/absui/ABSDealList')
        )
      },
      '/form-inspect': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSForm')
        )
      },
      '/countdown': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSCountdown')
        )
      },
      '/color-palette': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSColor')
        )
      },
      '/page-title': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSPageTitle')
        )
      },
      '/upload-avatar': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSUploadAvatar')
        )
      },
      '/double-date-picker': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSRangeDatePicker')
        )
      },
      '/sub-title': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, ['absui'], () =>
          import('../../views/absui/ABSSubTitle')
        )
      },
      '/link': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, [], () =>
          import('../../views/absui/ABSLink')
        )
      },
      '/upload-file': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, [], () =>
          import('../../views/absui/ABSUpload')
        )
      },
      '/file-list': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, [], () =>
          import('../../views/absui/ABSFileList')
        )
      },
      '/file-header': {
        identity: identity.common,
        component: routerUtil.dynamicWrapper(app, [], () =>
          import('../../views/absui/ABSFileHeader')
        )
      }
    };

    const SideBarLayout = routerConfig['/'].component;
    return (
      <Router history={this.props.history}>
        <Switch>
          <Redirect path="/" exact={true} to="/filtered-list" />
          <Route
            path="/"
            component={props => (
              <SideBarLayout
                menuKey={menuKey.absUI}
                routerConfig={routerConfig}
                {...props}
              />
            )}
          />
        </Switch>
      </Router>
    );
  }
}

/**
 * Dva Model Router Component Register and Bind
 * Every Router must set the model and component bind
 * use webpack chunk code split, you can set the chunk like the below
 * *webpackChunkName:'home'*
 * @param param0
 */
export function RouterConfig({ history, app }: any) {
  return <App history={history} app={app} />;
}
