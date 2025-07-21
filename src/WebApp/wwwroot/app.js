const { useState, useEffect } = React;

// API Service with better error handling
const ApiService = {
    async fetchData(url) {
        try {
            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return await response.json();
        } catch (error) {
            console.error('API Error:', error);
            throw error;
        }
    },

    async postData(url, data) {
        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data),
            });
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return await response.json();
        } catch (error) {
            console.error('API Error:', error);
            throw error;
        }
    }
};

// Service Status Component with enhanced UI
const ServiceStatus = ({ service, status, onRefresh }) => {
    const getStatusClass = (status) => {
        switch (status) {
            case 'healthy': return 'status-healthy';
            case 'error': return 'status-error';
            default: return 'status-loading';
        }
    };

    const getStatusIndicator = (status) => {
        switch (status) {
            case 'healthy': return 'status-indicator healthy';
            case 'error': return 'status-indicator error';
            default: return 'status-indicator loading';
        }
    };

    return (
        <div className="service-card">
            <div className="service-header">
                <div>
                    <span className={getStatusIndicator(status)}></span>
                    <i className={`fas fa-${service.icon} me-2`}></i>
                    {service.name}
                </div>
                <span className={`service-status ${getStatusClass(status)}`}>
                    {status === 'healthy' ? 'Healthy' : 
                     status === 'error' ? 'Error' : 'Loading...'}
                </span>
            </div>
            <div className="service-content">
                <button 
                    className="btn btn-outline-primary btn-sm"
                    onClick={onRefresh}
                    disabled={status === 'loading'}
                >
                    <i className="fas fa-sync-alt me-1"></i>
                    Refresh
                </button>
            </div>
        </div>
    );
};

// Product Management Component with enhanced error handling
const ProductManagement = () => {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [success, setSuccess] = useState(null);
    const [newProduct, setNewProduct] = useState({
        name: '',
        description: '',
        price: '',
        category: '',
        stockQuantity: ''
    });

    const loadProducts = async () => {
        setLoading(true);
        setError(null);
        try {
            const data = await ApiService.fetchData('http://localhost:5000/api/products');
            setProducts(data.data || []);
        } catch (error) {
            console.error('Failed to load products:', error);
            setError('Failed to load products. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    const createProduct = async (e) => {
        e.preventDefault();
        setError(null);
        setSuccess(null);
        try {
            await ApiService.postData('http://localhost:5000/api/products', newProduct);
            setNewProduct({ name: '', description: '', price: '', category: '', stockQuantity: '' });
            setSuccess('Product created successfully!');
            loadProducts();
        } catch (error) {
            console.error('Failed to create product:', error);
            setError('Failed to create product. Please try again.');
        }
    };

    useEffect(() => {
        loadProducts();
    }, []);

    return (
        <div className="service-card">
            <div className="service-header">
                <div>
                    <i className="fas fa-box me-2"></i>
                    Product Management
                </div>
            </div>
            <div className="service-content">
                {error && (
                    <div className="alert alert-danger mb-3">
                        <i className="fas fa-exclamation-triangle me-2"></i>
                        {error}
                    </div>
                )}
                {success && (
                    <div className="alert alert-success mb-3">
                        <i className="fas fa-check-circle me-2"></i>
                        {success}
                    </div>
                )}
                <div className="row">
                    <div className="col-md-6">
                        <h5>Add New Product</h5>
                        <form onSubmit={createProduct}>
                            <div className="mb-3">
                                <input
                                    type="text"
                                    className="form-control"
                                    placeholder="Product Name"
                                    value={newProduct.name}
                                    onChange={(e) => setNewProduct({...newProduct, name: e.target.value})}
                                    required
                                />
                            </div>
                            <div className="mb-3">
                                <textarea
                                    className="form-control"
                                    placeholder="Description"
                                    value={newProduct.description}
                                    onChange={(e) => setNewProduct({...newProduct, description: e.target.value})}
                                    required
                                />
                            </div>
                            <div className="row">
                                <div className="col-md-6">
                                    <input
                                        type="number"
                                        className="form-control"
                                        placeholder="Price"
                                        value={newProduct.price}
                                        onChange={(e) => setNewProduct({...newProduct, price: e.target.value})}
                                        required
                                    />
                                </div>
                                <div className="col-md-6">
                                    <input
                                        type="number"
                                        className="form-control"
                                        placeholder="Stock Quantity"
                                        value={newProduct.stockQuantity}
                                        onChange={(e) => setNewProduct({...newProduct, stockQuantity: e.target.value})}
                                        required
                                    />
                                </div>
                            </div>
                            <div className="mb-3">
                                <input
                                    type="text"
                                    className="form-control"
                                    placeholder="Category"
                                    value={newProduct.category}
                                    onChange={(e) => setNewProduct({...newProduct, category: e.target.value})}
                                    required
                                />
                            </div>
                            <button type="submit" className="btn btn-primary">
                                <i className="fas fa-plus me-1"></i>
                                Add Product
                            </button>
                        </form>
                    </div>
                    <div className="col-md-6">
                        <h5>Products ({products.length})</h5>
                        {loading ? (
                            <div className="text-center">
                                <div className="loading-spinner"></div>
                                <p>Loading products...</p>
                            </div>
                        ) : (
                            <div className="data-table">
                                {products.map((product, index) => (
                                    <div key={index} className="p-3 border-bottom">
                                        <div className="d-flex justify-content-between align-items-center">
                                            <div>
                                                <h6 className="mb-1">{product.name}</h6>
                                                <small className="text-muted">{product.description}</small>
                                            </div>
                                            <div className="text-end">
                                                <div className="fw-bold">${product.price}</div>
                                                <small className="text-muted">Stock: {product.stockQuantity}</small>
                                            </div>
                                        </div>
                                    </div>
                                ))}
                                {products.length === 0 && (
                                    <div className="p-3 text-center text-muted">
                                        <i className="fas fa-box-open fa-2x mb-2"></i>
                                        <p>No products found</p>
                                    </div>
                                )}
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};

// Order Management Component with enhanced error handling
const OrderManagement = () => {
    const [orders, setOrders] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const loadOrders = async () => {
        setLoading(true);
        setError(null);
        try {
            const data = await ApiService.fetchData('https://localhost:5005/api/orders');
            setOrders(data.data || []);
        } catch (error) {
            console.error('Failed to load orders:', error);
            setError('Failed to load orders. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadOrders();
    }, []);

    return (
        <div className="service-card">
            <div className="service-header">
                <div>
                    <i className="fas fa-shopping-cart me-2"></i>
                    Order Management
                </div>
            </div>
            <div className="service-content">
                {error && (
                    <div className="alert alert-danger mb-3">
                        <i className="fas fa-exclamation-triangle me-2"></i>
                        {error}
                    </div>
                )}
                <div className="d-flex justify-content-between align-items-center mb-3">
                    <h5>Orders ({orders.length})</h5>
                    <button className="btn btn-outline-primary btn-sm" onClick={loadOrders} disabled={loading}>
                        <i className="fas fa-sync-alt me-1"></i>
                        Refresh
                    </button>
                </div>
                {loading ? (
                    <div className="text-center">
                        <div className="loading-spinner"></div>
                        <p>Loading orders...</p>
                    </div>
                ) : (
                    <div className="data-table">
                        {orders.map((order, index) => (
                            <div key={index} className="p-3 border-bottom">
                                <div className="d-flex justify-content-between align-items-center">
                                    <div>
                                        <h6 className="mb-1">Order #{order.orderNumber}</h6>
                                        <small className="text-muted">
                                            Customer: {order.customerId} | Status: {order.status}
                                        </small>
                                    </div>
                                    <div className="text-end">
                                        <div className="fw-bold">${order.totalAmount}</div>
                                        <small className="text-muted">{new Date(order.orderDate).toLocaleDateString()}</small>
                                    </div>
                                </div>
                            </div>
                        ))}
                        {orders.length === 0 && (
                            <div className="p-3 text-center text-muted">
                                <i className="fas fa-shopping-cart fa-2x mb-2"></i>
                                <p>No orders found</p>
                            </div>
                        )}
                    </div>
                )}
            </div>
        </div>
    );
};

// Payment Management Component with enhanced error handling
const PaymentManagement = () => {
    const [payments, setPayments] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const loadPayments = async () => {
        setLoading(true);
        setError(null);
        try {
            const data = await ApiService.fetchData('https://localhost:5003/api/payments');
            setPayments(data.data || []);
        } catch (error) {
            console.error('Failed to load payments:', error);
            setError('Failed to load payments. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadPayments();
    }, []);

    return (
        <div className="service-card">
            <div className="service-header">
                <div>
                    <i className="fas fa-credit-card me-2"></i>
                    Payment Management
                </div>
            </div>
            <div className="service-content">
                {error && (
                    <div className="alert alert-danger mb-3">
                        <i className="fas fa-exclamation-triangle me-2"></i>
                        {error}
                    </div>
                )}
                <div className="d-flex justify-content-between align-items-center mb-3">
                    <h5>Payments ({payments.length})</h5>
                    <button className="btn btn-outline-primary btn-sm" onClick={loadPayments} disabled={loading}>
                        <i className="fas fa-sync-alt me-1"></i>
                        Refresh
                    </button>
                </div>
                {loading ? (
                    <div className="text-center">
                        <div className="loading-spinner"></div>
                        <p>Loading payments...</p>
                    </div>
                ) : (
                    <div className="data-table">
                        {payments.map((payment, index) => (
                            <div key={index} className="p-3 border-bottom">
                                <div className="d-flex justify-content-between align-items-center">
                                    <div>
                                        <h6 className="mb-1">Payment #{payment.paymentNumber}</h6>
                                        <small className="text-muted">
                                            Method: {payment.paymentMethod} | Status: {payment.status}
                                        </small>
                                    </div>
                                    <div className="text-end">
                                        <div className="fw-bold">${payment.amount}</div>
                                        <small className="text-muted">{new Date(payment.paymentDate).toLocaleDateString()}</small>
                                    </div>
                                </div>
                            </div>
                        ))}
                        {payments.length === 0 && (
                            <div className="p-3 text-center text-muted">
                                <i className="fas fa-credit-card fa-2x mb-2"></i>
                                <p>No payments found</p>
                            </div>
                        )}
                    </div>
                )}
            </div>
        </div>
    );
};

// Notification Management Component with enhanced error handling
const NotificationManagement = () => {
    const [notifications, setNotifications] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const loadNotifications = async () => {
        setLoading(true);
        setError(null);
        try {
            const data = await ApiService.fetchData('https://localhost:5007/api/notifications');
            setNotifications(data.data || []);
        } catch (error) {
            console.error('Failed to load notifications:', error);
            setError('Failed to load notifications. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadNotifications();
    }, []);

    return (
        <div className="service-card">
            <div className="service-header">
                <div>
                    <i className="fas fa-bell me-2"></i>
                    Notification Management
                </div>
            </div>
            <div className="service-content">
                {error && (
                    <div className="alert alert-danger mb-3">
                        <i className="fas fa-exclamation-triangle me-2"></i>
                        {error}
                    </div>
                )}
                <div className="d-flex justify-content-between align-items-center mb-3">
                    <h5>Notifications ({notifications.length})</h5>
                    <button className="btn btn-outline-primary btn-sm" onClick={loadNotifications} disabled={loading}>
                        <i className="fas fa-sync-alt me-1"></i>
                        Refresh
                    </button>
                </div>
                {loading ? (
                    <div className="text-center">
                        <div className="loading-spinner"></div>
                        <p>Loading notifications...</p>
                    </div>
                ) : (
                    <div className="data-table">
                        {notifications.map((notification, index) => (
                            <div key={index} className="p-3 border-bottom">
                                <div className="d-flex justify-content-between align-items-center">
                                    <div>
                                        <h6 className="mb-1">{notification.subject}</h6>
                                        <small className="text-muted">
                                            Type: {notification.type} | Status: {notification.status}
                                        </small>
                                    </div>
                                    <div className="text-end">
                                        <small className="text-muted">{new Date(notification.createdAt).toLocaleDateString()}</small>
                                    </div>
                                </div>
                            </div>
                        ))}
                        {notifications.length === 0 && (
                            <div className="p-3 text-center text-muted">
                                <i className="fas fa-bell fa-2x mb-2"></i>
                                <p>No notifications found</p>
                            </div>
                        )}
                    </div>
                )}
            </div>
        </div>
    );
};

// Main Dashboard Component with enhanced status checking
const Dashboard = () => {
    const [activeTab, setActiveTab] = useState('overview');
    const [servicesStatus, setServicesStatus] = useState({
        product: 'loading',
        order: 'loading',
        payment: 'loading',
        notification: 'loading'
    });

    const checkServiceHealth = async (serviceName, url) => {
        try {
            await ApiService.fetchData(`${url}/health`);
            setServicesStatus(prev => ({ ...prev, [serviceName]: 'healthy' }));
        } catch (error) {
            setServicesStatus(prev => ({ ...prev, [serviceName]: 'error' }));
        }
    };

    const checkAllServices = () => {
        checkServiceHealth('product', 'http://localhost:5000');
        checkServiceHealth('order', 'https://localhost:5005');
        checkServiceHealth('payment', 'https://localhost:5003');
        checkServiceHealth('notification', 'https://localhost:5007');
    };

    useEffect(() => {
        checkAllServices();
        const interval = setInterval(checkAllServices, 30000); // Check every 30 seconds
        return () => clearInterval(interval);
    }, []);

    const services = [
        { name: 'Product Service', icon: 'box', status: servicesStatus.product },
        { name: 'Order Service', icon: 'shopping-cart', status: servicesStatus.order },
        { name: 'Payment Service', icon: 'credit-card', status: servicesStatus.payment },
        { name: 'Notification Service', icon: 'bell', status: servicesStatus.notification }
    ];

    return (
        <div className="dashboard-container">
            <nav className="navbar navbar-expand-lg navbar-dark bg-dark mb-4">
                <div className="container-fluid">
                    <span className="navbar-brand">
                        <i className="fas fa-microchip me-2"></i>
                        E-Commerce Microservices Dashboard
                    </span>
                    <div className="navbar-nav ms-auto">
                        <span className="navbar-text">
                            <i className="fas fa-circle text-success me-1"></i>
                            .NET 9.0 Event-Driven Architecture
                        </span>
                    </div>
                </div>
            </nav>

            <div className="container-fluid">
                <ul className="nav nav-tabs mb-4" id="dashboardTabs">
                    <li className="nav-item">
                        <button 
                            className={`nav-link ${activeTab === 'overview' ? 'active' : ''}`}
                            onClick={() => setActiveTab('overview')}
                        >
                            <i className="fas fa-tachometer-alt me-1"></i>
                            Overview
                        </button>
                    </li>
                    <li className="nav-item">
                        <button 
                            className={`nav-link ${activeTab === 'products' ? 'active' : ''}`}
                            onClick={() => setActiveTab('products')}
                        >
                            <i className="fas fa-box me-1"></i>
                            Products
                        </button>
                    </li>
                    <li className="nav-item">
                        <button 
                            className={`nav-link ${activeTab === 'orders' ? 'active' : ''}`}
                            onClick={() => setActiveTab('orders')}
                        >
                            <i className="fas fa-shopping-cart me-1"></i>
                            Orders
                        </button>
                    </li>
                    <li className="nav-item">
                        <button 
                            className={`nav-link ${activeTab === 'payments' ? 'active' : ''}`}
                            onClick={() => setActiveTab('payments')}
                        >
                            <i className="fas fa-credit-card me-1"></i>
                            Payments
                        </button>
                    </li>
                    <li className="nav-item">
                        <button 
                            className={`nav-link ${activeTab === 'notifications' ? 'active' : ''}`}
                            onClick={() => setActiveTab('notifications')}
                        >
                            <i className="fas fa-bell me-1"></i>
                            Notifications
                        </button>
                    </li>
                </ul>

                <div className="tab-content">
                    {activeTab === 'overview' && (
                        <div>
                            <div className="row mb-4">
                                <div className="col-md-3">
                                    <div className="stats-card">
                                        <div className="stats-number">{services.filter(s => s.status === 'healthy').length}</div>
                                        <div className="stats-label">Healthy Services</div>
                                    </div>
                                </div>
                                <div className="col-md-3">
                                    <div className="stats-card">
                                        <div className="stats-number">{services.filter(s => s.status === 'error').length}</div>
                                        <div className="stats-label">Failed Services</div>
                                    </div>
                                </div>
                                <div className="col-md-3">
                                    <div className="stats-card">
                                        <div className="stats-number">4</div>
                                        <div className="stats-label">Total Services</div>
                                    </div>
                                </div>
                                <div className="col-md-3">
                                    <div className="stats-card">
                                        <div className="stats-number">Kafka</div>
                                        <div className="stats-label">Event Bus</div>
                                    </div>
                                </div>
                            </div>
                            <div className="row">
                                {services.map((service, index) => (
                                    <div key={index} className="col-md-6 col-lg-3 mb-3">
                                        <ServiceStatus 
                                            service={service}
                                            status={service.status}
                                            onRefresh={() => checkAllServices()}
                                        />
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}
                    {activeTab === 'products' && <ProductManagement />}
                    {activeTab === 'orders' && <OrderManagement />}
                    {activeTab === 'payments' && <PaymentManagement />}
                    {activeTab === 'notifications' && <NotificationManagement />}
                </div>
            </div>
        </div>
    );
};

// Render the app
ReactDOM.render(<Dashboard />, document.getElementById('root')); 