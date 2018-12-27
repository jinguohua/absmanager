import React from 'react';
import { IABSSidebarMenuData } from '../../components/ABSSidebar';
import AuthLayout from './Layout';

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

  class ABSAuthDecorator extends React.Component<IAnonymousRootLayoutProps> {

    componentDidMount() {
      // console.log('authDecorator did mount');
    }

    componentWillReceiveProps() {
       // console.log('authDecorator did mount');
    }

    render() {
      return (
        <AuthLayout>
          <WrappedComponent {...this.props}/>
        </AuthLayout>
      );
    }
  };

export default extendsRootLayout;