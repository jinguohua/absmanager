import RootLayout from './Layout';
import React from 'react';
import { IABSSidebarMenuData } from '../../components/ABSSidebar';
import NotFound from '../ABSNotFound';

interface IAnonymousRootLayoutProps {
  menus: Array<IABSSidebarMenuData>; // product
  match: any; // product
  routerConfig: any;
  dispatch: any;
  menu: any;
  user: any;
  notice: any;
  pageHeader: React.ReactNode;
  navigationMenus: any; // global
}

const extendsRootLayout = () => WrappedComponent =>

  class ABSBaseDecorator extends React.Component<IAnonymousRootLayoutProps> {

    componentDidMount() {
      // console.log('decorator did mount');
    }

    componentWillReceiveProps(nextProps: any) {
      // console.log('decorator did mount');
    }

    notFound = () => (
      <NotFound />
    )

    render() {
      return (
        <RootLayout>
          <WrappedComponent {...this.props} />
        </RootLayout>
      );
    }
  };

export default extendsRootLayout;